using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            // Créer une instance de la fumée à la position de l'explosion
            GameObject smokeInstance = Instantiate(smokeParticlesPrefab.gameObject, transform.position, Quaternion.identity);

            // Démarrer les particules
            ParticleSystem ps = smokeInstance.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            // Ajouter ou configurer la zone de blocage
            SmokeZone sz = smokeInstance.GetComponent<SmokeZone>();
            if (sz == null)
                sz = smokeInstance.AddComponent<SmokeZone>();

            sz.Initialize(smokeDuration, turretLayer);

            // Supprimer la fumée et la zone après sa durée
            StartCoroutine(DestroySmokeAfterDuration(smokeInstance, smokeDuration));
        }

        // Désactiver l'objet explosif après l'explosion
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
    private float smokeDuration;
    private LayerMask turretLayer;

    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    public void Initialize(float duration, LayerMask layerMask)
    {
        smokeDuration = duration;
        turretLayer = layerMask;

        StartCoroutine(ZoneLifetime());
    }

    private IEnumerator ZoneLifetime()
    {
        yield return new WaitForSeconds(smokeDuration);
        RestoreAllAffectedLayers();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, turretLayer))
        {
            GameObject obj = other.gameObject;
            if (!affectedObjects.Contains(obj))
            {
                originalLayers[obj] = obj.layer;
                obj.layer = LayerMask.NameToLayer("Smoke");
                affectedObjects.Add(obj);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (affectedObjects.Contains(obj))
        {
            RestoreLayer(obj);
            affectedObjects.Remove(obj);
        }
    }

    private void RestoreAllAffectedLayers()
    {
        foreach (var obj in affectedObjects)
        {
            RestoreLayer(obj);
        }
        affectedObjects.Clear();
    }

    private void RestoreLayer(GameObject obj)
    {
        if (obj != null && originalLayers.ContainsKey(obj))
        {
            obj.layer = originalLayers[obj];
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
