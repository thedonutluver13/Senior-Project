using UnityEngine;
using Mirror;

public class movement : NetworkBehaviour
{
    public float speed = 5f;

    [Header("DUAL Arena Bounds")]
    public float minX = -8f;
    public float maxX =  8f;
    public float minY = -4.5f;
    public float maxY =  4.5f;
    public float seamY = 0f;
    public float seamMargin = 0.25f;

    [Header("Shooting")]
    public GameObject bulletPrefab;     // assign in Inspector
    public Transform firePoint;         // assign in Inspector (empty child)
    public float bulletSpeed = 12f;
    public float fireCooldown = 0.25f;

    private Rigidbody2D rb;
    private float nextFireTime;

    public enum Side { Bottom, Top }

    [SyncVar] public Side side = Side.Bottom;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer fired!");

        // Optional: flip camera for the Top player so both feel like "I'm at bottom"
        // If you don't want camera flipping yet, comment this out.
        if (Camera.main != null && side == Side.Top)
        {
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180f);
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (GameManager.Instance != null && GameManager.Instance.gameOver)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (side == Side.Top)
        {
            x = -x;
            y = -y;
        }

        // NOTE: In most Unity versions this is rb.velocity, not rb.linearVelocity.
        // If rb.linearVelocity compiles for you, fine. Otherwise change to rb.velocity.
        // rb.linearVelocity works!
        rb.linearVelocity = new Vector2(x, y).normalized * speed;

        // clamp locally for feel (server will also clamp via NetworkTransform updates)
        ClampToSideLocal();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (GameManager.Instance != null && GameManager.Instance.gameOver)
            return;

        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            CmdShoot();
        }
    }

    [Command]
    private void CmdShoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Bottom shoots up, Top shoots down
        Vector2 dir = (side == Side.Bottom) ? Vector2.up : Vector2.down;

        b.GetComponent<Bullet>().ServerInit(dir, bulletSpeed);

        NetworkServer.Spawn(b);
    }

    private void ClampToSideLocal()
    {
        Vector2 pos = rb.position;

        // clamp X for everyone
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        // clamp Y based on side
        if (side == Side.Bottom)
        {
            pos.y = Mathf.Clamp(pos.y, minY, seamY - seamMargin);
        }
        else
        {
            pos.y = Mathf.Clamp(pos.y, seamY + seamMargin, maxY);
        }

        rb.position = pos;
    }
}