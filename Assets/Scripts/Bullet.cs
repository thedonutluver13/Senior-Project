using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float lifeTime = 2.0f;

    private Vector2 dir = Vector2.up;
    private float speed = 12f;

    // Called on server after spawn
    [Server]
    public void ServerInit(Vector2 direction, float bulletSpeed)
    {
        dir = direction.normalized;
        speed = bulletSpeed;

        Invoke(nameof(ServerDespawn), lifeTime);
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        transform.position += (Vector3)(dir * speed * Time.fixedDeltaTime);
    }

    [Server]
    private void ServerDespawn()
    {
        if (gameObject != null)
            NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we hit a player, damage them
        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        
        if (health != null)
        {
            health.TakeDamage(1);
        }

        NetworkServer.Destroy(gameObject);
}
}