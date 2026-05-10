using UnityEngine;
using TMPro;
using Mirror;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        resultText.gameObject.SetActive(false);
    }

    public void ShowResult(uint winnerNetId)
    {
        if (NetworkClient.localPlayer == null) return;

        resultText.gameObject.SetActive(true);

        if (NetworkClient.localPlayer.netId == winnerNetId)
            resultText.text = "YOU WIN";
        else
            resultText.text = "YOU LOSE";
    }
}