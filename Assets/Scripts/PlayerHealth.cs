using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] public int health = 1;

    [Server]
    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        health -= amount;

        if (health <= 0)
        {
            Debug.Log($"Player {netId} died");

            // Find the other player as the winner
            foreach (var kv in NetworkServer.spawned)
            {
                var player = kv.Value.GetComponent<PlayerHealth>();
                if (player != null && player.netId != netId)
                {
                    GameManager.Instance.ServerDeclareWinner(player.netId);
                    break;
                }
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}