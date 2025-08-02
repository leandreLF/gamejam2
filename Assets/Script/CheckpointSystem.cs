using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Transform checkpointTransform;
    public GameObject activatedVisual;
    public GameObject deactivatedVisual;
    public AudioClip activationSound;
    public ParticleSystem activationParticles;

    private bool isActivated = false; // Déclaration correcte de la variable

    private void Start()
    {
        UpdateVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            ActivateCheckpoint(other);
        }
    }

    private void ActivateCheckpoint(Collider player)
    {
        isActivated = true;
        UpdateVisuals();

        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }

        if (activationParticles != null)
        {
            activationParticles.Play();
        }

        PlayerRespawn playerRespawn = player.GetComponentInParent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.SetSpawnPoint(checkpointTransform);
        }
    }

    private void UpdateVisuals()
    {
        if (activatedVisual != null)
        {
            activatedVisual.SetActive(isActivated);
        }

        if (deactivatedVisual != null)
        {
            deactivatedVisual.SetActive(!isActivated);
        }
    }

    public void ResetCheckpoint()
    {
        isActivated = false;
        UpdateVisuals();
    }
}