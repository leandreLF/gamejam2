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

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        initialLayer = gameObject.layer;
        initialTag = gameObject.tag;

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
            health.ResetHealth();  // Remise � z�ro compl�te du health et �tats li�s
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;          // Correction ici
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearDamping = 0f;                        // Correction ici
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; // adapte selon besoin
        }

        gameObject.layer = initialLayer;
        gameObject.tag = initialTag;

        ResetAnimation();
    }

    protected virtual void ResetAnimation()
    {
        if (animator != null && animator.enabled)
        {
            animator.Rebind();
            animator.Update(0f);
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
