using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string roomName;
        public Transform[] spawnPoints;
        public List<ResettableObject> roomObjects = new List<ResettableObject>();
        public Transform roomCameraPosition;
    }

    [Header("Room Settings")]
    public Room[] rooms;
    public int currentRoomIndex = 0;

    [Header("References")]
    public Camera mainCamera;
    public GameObject playerPrefab;

    private GameObject currentPlayer;

    public static RoomManager Instance { get; private set; }

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
        InitializeRoomSystem();
        SpawnPlayer();
    }

    private void InitializeRoomSystem()
    {
        foreach (Room room in rooms)
        {
            foreach (ResettableObject obj in room.roomObjects)
            {
                if (obj != null) obj.SetInitialState();
            }
        }
    }

    public void SpawnPlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer);

        Room currentRoom = rooms[currentRoomIndex];
        Transform spawnPoint = currentRoom.spawnPoints[0];
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        PositionCamera();
    }

    private void PositionCamera()
    {
        Room currentRoom = rooms[currentRoomIndex];
        if (currentRoom.roomCameraPosition != null)
        {
            mainCamera.transform.position = currentRoom.roomCameraPosition.position;
            mainCamera.transform.rotation = currentRoom.roomCameraPosition.rotation;
        }
    }

    public void ResetCurrentRoom()
    {
        Room currentRoom = rooms[currentRoomIndex];

        foreach (ResettableObject resettable in currentRoom.roomObjects)
        {
            if (resettable != null) resettable.ResetObject();
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowReadyUI();
        }

        ResetCheckpoints();

        PlayerRespawn playerRespawn = currentPlayer.GetComponent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.ResetPlayer();
        }
        else
        {
            Transform spawnPoint = currentRoom.spawnPoints[0];
            currentPlayer.transform.position = spawnPoint.position;
            currentPlayer.transform.rotation = spawnPoint.rotation;
        }

        PositionCamera();
    }

    public void ChangeRoom(int newRoomIndex)
    {
        if (newRoomIndex < 0 || newRoomIndex >= rooms.Length) return;

        currentRoomIndex = newRoomIndex;
        Room newRoom = rooms[currentRoomIndex];

        Transform spawnPoint = newRoom.spawnPoints[0];
        currentPlayer.transform.position = spawnPoint.position;
        currentPlayer.transform.rotation = spawnPoint.rotation;

        PositionCamera();
    }

    public void RegisterObjectInCurrentRoom(GameObject obj)
    {
        ResettableObject resettable = obj.GetComponent<ResettableObject>();
        if (resettable != null)
        {
            rooms[currentRoomIndex].roomObjects.Add(resettable);
            resettable.SetInitialState();
        }
    }

    public void ResetCheckpoints()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in allCheckpoints)
        {
            if (checkpoint != null) checkpoint.ResetCheckpoint();
        }
    }
}