using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Ready UI")]
    public GameObject readyUIContainer;
    public Button readyButton;

    [Header("Message Display")]
    public Text messageTextUI;
    public CanvasGroup messageCanvasGroup;
    public float messageFadeDuration = 0.2f;

    private Coroutine messageCoroutine;

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

        if (messageCanvasGroup != null)
        {
            messageCanvasGroup.alpha = 0;
        }
    }

    void Start()
    {
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(OnReadyButtonPressed);
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

    public void OnReadyButtonPressed()
    {
        List<ResettableObject> roomObjects = RoomManager.Instance.GetCurrentRoomObjects();

        for (int i = roomObjects.Count - 1; i >= 0; i--)
        {
            var item = roomObjects[i];
            if (item == null)
            {
                roomObjects.RemoveAt(i);
            }
            else
            {
                item.UpdateInitialStateToCurrent();
            }
        }

        HideReadyUI();
    }

    public void ShowMessage(string message, float duration)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(DisplayMessage(message, duration));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        messageTextUI.text = message;
        yield return FadeCanvasGroup(messageCanvasGroup, 1f, messageFadeDuration);

        yield return new WaitForSeconds(duration);

        yield return FadeCanvasGroup(messageCanvasGroup, 0f, messageFadeDuration);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
    {
        float startAlpha = group.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        group.alpha = targetAlpha;
    }
}
