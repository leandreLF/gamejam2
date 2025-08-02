using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ResettableObject : MonoBehaviour
{
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    protected Vector3 initialScale;
    protected bool initialActiveState;
    protected Animator animator;
    protected string initialAnimationState;
    protected float initialAnimationTime;

    protected Rigidbody rb;
    protected Collider col;
    protected int initialLayer;
    protected string initialTag;

    public virtual void SetInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        initialActiveState = gameObject.activeSelf;

        // Enregistrer la physique
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Enregistrer l'�tat du layer et tag (utile pour grab/d�tection)
        initialLayer = gameObject.layer;
        initialTag = gameObject.tag;

        // Animation
        animator = GetComponent<Animator>();
        if (animator != null && animator.enabled)
        {
            initialAnimationState = GetCurrentAnimationState();
            initialAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }

    public virtual void ResetObject()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        gameObject.SetActive(initialActiveState);
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.Revive();
        }

        // Restaurer la physique
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearDamping = 0f;
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; // � adapter selon le design
        }

        // Restaurer tag et layer
        gameObject.layer = initialLayer;
        gameObject.tag = initialTag;

        // R�initialiser l'animation
        ResetAnimation();
    }

    protected virtual void ResetAnimation()
    {
        if (animator != null && animator.enabled)
        {
            animator.enabled = true;

            if (!string.IsNullOrEmpty(initialAnimationState))
            {
                animator.Play(initialAnimationState, 0, initialAnimationTime);
            }
            else
            {
                animator.Rebind();
                animator.Update(0f);
            }
        }
    }

    private string GetCurrentAnimationState()
    {
        if (animator != null && animator.enabled && animator.runtimeAnimatorController != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                return clipInfo[0].clip.name;
            }
        }
        return "";
    }
    public void UpdateInitialStateToCurrent()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        initialActiveState = gameObject.activeSelf;
    }
}
