using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.items.Add(this);
        }
    }

    public void ResetItem()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.items.Remove(this);
        }
    }
}