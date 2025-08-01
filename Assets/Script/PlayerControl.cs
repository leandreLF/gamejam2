using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Health health;
    private bool isFrozen = true;

    void Start() => health = GetComponent<Health>();

    void Update()
    {
        if (isFrozen) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime);
    }

    public void Unfreeze() => isFrozen = false;
}