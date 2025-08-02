using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Text deathMessage;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverPanel.alpha = 0;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;

        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowGameOver()
    {
        string[] messages = {
            "Bob a explosé!",
            "Mission échouée!",
            "Retour à la case départ",
            "Bob n'est plus..."
        };
        deathMessage.text = messages[Random.Range(0, messages.Length)];

        StartCoroutine(FadePanel(0, 1, true));
    }

    public void RestartGame()
    {
        StartCoroutine(FadePanel(1, 0, false));

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ResetCurrentRoom();
        }
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator FadePanel(float startAlpha, float endAlpha, bool makeInteractive)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            gameOverPanel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameOverPanel.alpha = endAlpha;
        gameOverPanel.interactable = makeInteractive;
        gameOverPanel.blocksRaycasts = makeInteractive;
    }
}