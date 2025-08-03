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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeRooms();
        SpawnPlayer();
    }

    private void InitializeRooms()
    {
        foreach (Room room in rooms)
        {
            foreach (ResettableObject obj in room.roomObjects)
            {
                if (obj != null)
                {
                    obj.SetInitialState();
                }
            }
        }
    }

    public void SpawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        Transform spawn = GetCurrentRoom().spawnPoints[0];
        currentPlayer = Instantiate(playerPrefab, spawn.position, spawn.rotation);

        // Récupère le PlayerRespawn et mets à jour son spawn point
        PlayerRespawn playerRespawn = currentPlayer.GetComponent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.SetSpawnPoint(spawn);
        }

        PositionCamera();
    }

    private void PositionCamera()
    {
        Transform camPos = GetCurrentRoom().roomCameraPosition;
        if (camPos != null)
        {
            mainCamera.transform.position = camPos.position;
            mainCamera.transform.rotation = camPos.rotation;
        }
    }

    public void ResetCurrentRoom()
    {
        foreach (ResettableObject obj in GetCurrentRoom().roomObjects)
        {
            if (obj != null)
                obj.ResetObject();
        }

        UIManager.Instance?.ShowReadyUI();

        ResetCheckpoints();

        var playerRespawn = currentPlayer.GetComponent<PlayerRespawn>();
        Transform spawn = GetCurrentRoom().spawnPoints[0];

        if (playerRespawn != null)
        {
            playerRespawn.SetSpawnPoint(spawn); // **Important**
            playerRespawn.ResetPlayer();
        }
        else
        {
            currentPlayer.transform.position = spawn.position;
            currentPlayer.transform.rotation = spawn.rotation;
        }

        PositionCamera();
    }


    public void ChangeRoom(int newRoomIndex)
    {
        if (newRoomIndex < 0 || newRoomIndex >= rooms.Length) return;

        currentRoomIndex = newRoomIndex;
        Transform spawn = GetCurrentRoom().spawnPoints[0];

        currentPlayer.transform.position = spawn.position;
        currentPlayer.transform.rotation = spawn.rotation;

        PositionCamera();
    }

    public void RegisterObjectInCurrentRoom(GameObject obj)
    {
        ResettableObject resettable = obj.GetComponent<ResettableObject>();
        if (resettable != null && !GetCurrentRoom().roomObjects.Contains(resettable))
        {
            GetCurrentRoom().roomObjects.Add(resettable);
            resettable.SetInitialState();
        }
    }

    public void OnReadyPressed()
    {
        foreach (ResettableObject obj in GetCurrentRoom().roomObjects)
        {
            if (obj != null)
                obj.UpdateInitialStateToCurrent();
        }
    }

    public List<ResettableObject> GetCurrentRoomObjects()
    {
        return GetCurrentRoom().roomObjects;
    }

    public void ResetCheckpoints()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in allCheckpoints)
        {
            checkpoint?.ResetCheckpoint();
        }
    }

    private Room GetCurrentRoom()
    {
        return rooms[currentRoomIndex];
    }
}
