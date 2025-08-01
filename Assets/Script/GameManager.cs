using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button readyButton;
    public PlayerController player;

    void Start()
    {
        readyButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        player.Unfreeze();
        readyButton.gameObject.SetActive(false);
    }
}