using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public int targetRoomIndex = -1;
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && targetRoomIndex >= 0)
        {
            RoomManager.Instance.ChangeRoom(targetRoomIndex);
        }
    }
}