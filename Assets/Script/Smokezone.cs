using UnityEngine;

public class SmokeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TurretEnemy turret = other.GetComponent<TurretEnemy>();
        if (turret != null)
        {
            turret.SetInSmoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TurretEnemy turret = other.GetComponent<TurretEnemy>();
        if (turret != null)
        {
            turret.SetInSmoke(false);
        }
    }
}
