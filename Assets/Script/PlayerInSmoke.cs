using UnityEngine;

public class PlayerInSmoke : MonoBehaviour
{
    public bool IsInSmoke { get; private set; }

    public void SetInSmoke(bool value)
    {
        IsInSmoke = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SmokeZone>() != null)
            SetInSmoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SmokeZone>() != null)
            SetInSmoke(false);
    }
}
