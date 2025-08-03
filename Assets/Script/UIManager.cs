using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
}
