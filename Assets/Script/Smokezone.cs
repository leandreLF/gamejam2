using UnityEngine;
using System.Collections;

public class SmokeZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float smokeDuration = 3f;
    [SerializeField] private LayerMask turretLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer, turretLayer))
        {
            // Change le layer vers "Smoke" pendant la durée de l'effet
            other.gameObject.layer = LayerMask.NameToLayer("Smoke");
            StartCoroutine(RemoveSmokeEffect(other.gameObject));
        }
    }

    private IEnumerator RemoveSmokeEffect(GameObject turret)
    {
        yield return new WaitForSeconds(smokeDuration);
        if (turret != null)
        {
            turret.layer = LayerMask.NameToLayer("Default");
        }
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}