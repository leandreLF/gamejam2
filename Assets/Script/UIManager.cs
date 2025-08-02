using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Ready UI")]
    public GameObject readyUIContainer;
    public Button readyButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowReadyUI()
    {
        if (readyUIContainer != null)
        {
            readyUIContainer.SetActive(true);
        }
    }

    public void HideReadyUI()
    {
        if (readyUIContainer != null)
        {
            readyUIContainer.SetActive(false);
        }
    }
}