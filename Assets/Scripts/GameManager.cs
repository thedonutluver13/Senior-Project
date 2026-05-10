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
}