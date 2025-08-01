using UnityEngine;
using UnityEngine.UI;

public class RailMover : MonoBehaviour
{
    [Header("Rail Points")]
    public Transform[] railPoints;
    public float speed = 5f;
    private int currentPointIndex = 0;

    [Header("State")]
    public bool isFrozen = true;
    private bool isMoving = false;

    [Header("Kick Settings")]
    public float kickDamage = 20f;
    public float kickCooldown = 1f;
    public float obstacleDetectionDistance = 1f;
    public LayerMask obstacleLayer;
    private bool canKick = true;

    [Header("UI")]
    public Button readyButton;

    [Header("Animation")]
    public Animator animator;

    [Header("Gizmos")]
    public Color gizmoColor = Color.green;
    public float gizmoSphereRadius = 0.2f;

    void Start()
    {
        if (readyButton != null)
            readyButton.onClick.AddListener(Ready);

        UpdateAnimator();
    }

    void Update()
    {
        if (!isMoving || isFrozen || currentPointIndex >= railPoints.Length)
        {
            UpdateAnimator();
            return;
        }

        if (DetectObstacle())
        {
            Freeze();
            Kick();
            return;
        }

        Transform targetPoint = railPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= railPoints.Length)
            {
                isMoving = false;
                ResetPlayer(); // Réinitialiser l'état et réafficher le bouton
            }
        }

        UpdateAnimator();
    }

    public void Ready()
    {
        isFrozen = false;
        isMoving = true;
        if (readyButton != null)
            readyButton.gameObject.SetActive(false);
        UpdateAnimator();
    }

    public void Freeze()
    {
        isFrozen = true;
        isMoving = false;
        UpdateAnimator();
    }

    public void ResetPlayer()
    {
        currentPointIndex = 0;
        transform.position = railPoints[0].position;
        isFrozen = true;
        isMoving = false;

        if (readyButton != null)
            readyButton.gameObject.SetActive(true); // Réaffiche le bouton

        UpdateAnimator();
    }

    public void Kick()
    {
        if (!canKick) return;

        animator?.SetTrigger("isKicking");
        canKick = false;

        // Ici, tu pourrais appliquer des dégâts à l’obstacle si nécessaire

        Invoke(nameof(ResetKick), kickCooldown);
    }

    void ResetKick()
    {
        canKick = true;
    }

    private bool DetectObstacle()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.forward, out hit, obstacleDetectionDistance, obstacleLayer);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool walking = isMoving && !isFrozen;
        bool standing = isFrozen;

        animator.SetBool("isMoving", walking);
        animator.SetBool("isStanding", standing);

        // isKicking est un Trigger
    }

    void OnDrawGizmos()
    {
        if (railPoints == null || railPoints.Length == 0)
            return;

        Gizmos.color = gizmoColor;

        for (int i = 0; i < railPoints.Length; i++)
        {
            if (railPoints[i] != null)
            {
                Gizmos.DrawSphere(railPoints[i].position, gizmoSphereRadius);

                if (i < railPoints.Length - 1 && railPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(railPoints[i].position, railPoints[i + 1].position);
                }
            }
        }

        // Raycast obstacle
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleDetectionDistance);
    }
}
