using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float transitionSpeed = 5f;
    public Vector3 roomOffset = new Vector3(0, 10f, -10f);

    private Vector3 targetPosition;
    private bool isTransitioning;

    void Update()
    {
        if (isTransitioning)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isTransitioning = false;
            }
        }
    }

    public void SwitchToRoom(Vector3 roomCenter)
    {
        targetPosition = roomCenter + roomOffset;
        isTransitioning = true;
    }
}