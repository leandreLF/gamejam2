using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.items.Add(this);
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearDamping = 0f;
        }

        // R�activer le collider si besoin
        if (col != null)
        {
            col.enabled = true;
        }
    }

    public void ResetItem()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        gameObject.SetActive(true);

        if (rb == null) rb = GetComponent<Rigidbody>();
        if (col == null) col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearDamping = 0f;
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; // selon ton système
        }

        // RÉTABLIR le bon layer et tag
        gameObject.layer = LayerMask.NameToLayer("Grabbable"); // ou "Default", selon ton système
        gameObject.tag = "enemy"; // vérifie le tag attendu par ta turret
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.items.Remove(this);
        }
    }
}