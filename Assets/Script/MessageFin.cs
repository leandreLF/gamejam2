using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [TextArea]
    public string message = "Vous êtes entré dans une nouvelle zone.";
    public float displayDuration = 3f;
    public string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            UIManager.Instance?.ShowMessage(message, displayDuration);
        }
    }
}
