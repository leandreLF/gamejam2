using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public GameObject smokeZonePrefab; // Prefab de la zone de fumée (avec SmokeZone.cs)
    public float delayBeforeSmoke = 1f; // Délai avant activation de la fumée
    public float smokeDuration = 5f;    // Durée de la fumée

    private bool hasExploded = false;

    void Start()
    {
        Invoke(nameof(ActivateSmoke), delayBeforeSmoke);
    }

    void ActivateSmoke()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Crée la zone de fumée à la position de la grenade
        GameObject smoke = Instantiate(smokeZonePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, smokeDuration); // Supprime la zone de fumée après X secondes

        Destroy(gameObject); // Détruit la grenade elle-même
    }
}
