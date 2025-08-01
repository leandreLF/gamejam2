using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public ParticleSystem activationEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Active le checkpoint
            CheckpointSystem.Instance.ActivateCheckpoint(transform);

            // Effet visuel
            if (activationEffect != null)
                activationEffect.Play();

            // Désactive le collider pour éviter de retrigger
            GetComponent<Collider>().enabled = false;
        }
    }
}