using UnityEngine;

public class ResettableEnemy : ResettableObject
{
    private Health enemyHealth;
    private EnemyController enemyController;
    private TurretEnemy turretEnemy;

    protected override void ResetAnimation()
    {
        base.ResetAnimation();

        if (enemyController != null)
        {
            enemyController.ResetEnemyState();
        }
    }

    public override void SetInitialState()
    {
        base.SetInitialState();

        enemyHealth = GetComponent<Health>();
        enemyController = GetComponent<EnemyController>();
        turretEnemy = GetComponent<TurretEnemy>();
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }

        if (turretEnemy != null)
        {
            turretEnemy.enabled = true;
            turretEnemy.ActivateTurret();
    }
}
}
