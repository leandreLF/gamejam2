using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public GameObject smokeZonePrefab; // Prefab de la zone de fum�e (avec SmokeZone.cs)
    public float delayBeforeSmoke = 1f; // D�lai avant activation de la fum�e
    public float smokeDuration = 5f;    // Dur�e de la fum�e

    private bool hasExploded = false;

    void Start()
    {
        Invoke(nameof(ActivateSmoke), delayBeforeSmoke);
    }

    void ActivateSmoke()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Cr�e la zone de fum�e � la position de la grenade
        GameObject smoke = Instantiate(smokeZonePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, smokeDuration); // Supprime la zone de fum�e apr�s X secondes

        Destroy(gameObject); // D�truit la grenade elle-m�me
    }
}
