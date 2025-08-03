using UnityEngine;
using System.Collections;

public class SmokeExplosive : ResettableObject
{
    [SerializeField] private ParticleSystem smokeParticlesPrefab;
    [SerializeField] private float smokeDuration = 3f;
    [SerializeField] private LayerMask turretLayer;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health != null)
            health.OnDeath += Explode;
    }

    private void Explode()
    {
        if (smokeParticlesPrefab != null)
        {
            // Instancier la fumée
            GameObject smokeInstance = Instantiate(smokeParticlesPrefab.gameObject, transform.position, transform.rotation);

            // Lancer la particule
            ParticleSystem ps = smokeInstance.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            // Ajouter ou récupérer le SmokeZone sur le prefab instancié
            SmokeZone sz = smokeInstance.GetComponent<SmokeZone>();
            if (sz == null)
            {
                sz = smokeInstance.AddComponent<SmokeZone>();
            }

            sz.smokeDuration = smokeDuration;
            sz.turretLayer = turretLayer;

            StartCoroutine(DestroySmokeAfterDuration(smokeInstance, ps.main.duration));
        }

        gameObject.SetActive(false);
    }

    private IEnumerator DestroySmokeAfterDuration(GameObject smokeInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (smokeInstance != null)
            Destroy(smokeInstance);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (health != null)
            health.ResetHealth();

        gameObject.SetActive(true);
    }
}

public class SmokeZone : MonoBehaviour
{
    [HideInInspector] public float smokeDuration = 3f;
    [HideInInspector] public LayerMask turretLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, turretLayer))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Smoke");
            StartCoroutine(RemoveSmokeEffect(other.gameObject));
        }
    }

    private IEnumerator RemoveSmokeEffect(GameObject turret)
    {
        yield return new WaitForSeconds(smokeDuration);
        if (turret != null)
            turret.layer = LayerMask.NameToLayer("Default");
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
