using UnityEngine;

public class ResettableEnemy : ResettableObject
{
    private Health enemyHealth;
    private EnemyController enemyController;

    protected override void ResetAnimation()
    {
        base.ResetAnimation();

        // Réinitialisation spécifique aux ennemis
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
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }
    }
}