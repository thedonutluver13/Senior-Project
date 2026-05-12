using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SyncVar] public bool gameOver = false;
    [SyncVar] public uint winnerNetId = 0;

    private GameOverUI gameOverUI;

    void Awake()
    {
        Instance = this;
        gameOverUI = FindFirstObjectByType<GameOverUI>();
    }

    [Server]
    public void ServerDeclareWinner(uint winner)
    {
        if (gameOver) return;

        gameOver = true;

        foreach (var kv in NetworkServer.spawned)
        {
            var m = kv.Value.GetComponent<movement>(); //disables lpayers from moving after someone dies
            if (m != null)
                m.enabled = false;

            var rb = kv.Value.GetComponent<Rigidbody2D>(); //force velocity to go to 0
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;   // or rb.velocity = Vector2.zero (depending on your Unity version)
                rb.angularVelocity = 0f;
            }
        }


        winnerNetId = winner;

        Debug.Log($"Game Over. Winner netId = {winnerNetId}");

        RpcGameOver(winnerNetId);
    }

    [ClientRpc]
    void RpcGameOver(uint winner)
    {
       Debug.Log($"[CLIENT] Game Over. Winner netId = {winner}");

        if (gameOverUI != null)
            gameOverUI.ShowResult(winner);
    }
    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}