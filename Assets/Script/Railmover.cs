using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class RailMover : MonoBehaviour
{
    // Événements statiques
    public static event UnityAction<bool> OnFreezeStateChanged;
    public static event UnityAction OnGameStarted;

    [Header("Rail Settings")]
    [SerializeField] private Transform[] railPoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waypointThreshold = 0.1f;

    [Header("UI Settings")]
    [SerializeField] private GameObject readyUIContainer;
    [SerializeField] private Button readyButton;

    [Header("Combat Settings")]
    [SerializeField] private float kickDamage = 20f;
    [SerializeField] private float kickCooldown = 1f;
    [SerializeField] private float obstacleDetectionRange = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    // Références internes
    private Health health;
    private Animator animator;
    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool canKick = true;

    // Propriétés
    public bool IsFrozen { get; private set; } = true;

    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        // Initialisation sécurisée
        if (readyButton == null || readyUIContainer == null)
        {
            Debug.LogError("UI references missing in RailMover!", this);
            enabled = false;
            return;
        }

        readyButton.onClick.AddListener(OnReadyPressed);
        health.OnDeath += OnPlayerDeath;
    }

    void Update()
    {
        if (IsFrozen || !isMoving) return;

        HandleMovement();
        HandleObstacleDetection();
    }

    void HandleMovement()
    {
        if (currentPointIndex >= railPoints.Length)
        {
            EndPath();
            return;
        }

        Transform target = railPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) <= waypointThreshold)
        {
            currentPointIndex++;
        }
    }

    void HandleObstacleDetection()
    {
        if (!canKick) return;

        if (Physics.Raycast(transform.position, transform.forward, out var hit, obstacleDetectionRange, obstacleLayer))
        {
            PerformKick();
            if (hit.transform.TryGetComponent<Health>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(kickDamage);
            }
        }
    }

    void PerformKick()
    {
        canKick = false;
        animator?.SetTrigger("Kick");
        Invoke(nameof(ResetKick), kickCooldown);
    }

    void ResetKick() => canKick = true;

    void OnReadyPressed()
    {
        readyUIContainer.SetActive(false);
        StartMovement();
    }

    void StartMovement()
    {
        IsFrozen = false;
        isMoving = true;
        OnFreezeStateChanged?.Invoke(false);
        OnGameStarted?.Invoke();
        UpdateAnimator();
    }

    void OnPlayerDeath()
    {
        ResetPlayer();
    }

    public void ResetPlayer()
    {
        currentPointIndex = 0;
        transform.position = railPoints[0].position;
        IsFrozen = true;
        isMoving = false;

        readyUIContainer.SetActive(true);
        health.ResetHealth();
        UpdateAnimator();
    }

    void EndPath()
    {
        isMoving = false;
        Debug.Log("Path completed!");
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetBool("IsMoving", isMoving && !IsFrozen);
    }

    void OnDestroy()
    {
        health.OnDeath -= OnPlayerDeath;
        if (readyButton != null)
            readyButton.onClick.RemoveListener(OnReadyPressed);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Visualisation du rail
        Gizmos.color = Color.blue;
        for (int i = 0; i < railPoints.Length; i++)
        {
            if (railPoints[i] == null) continue;
            Gizmos.DrawSphere(railPoints[i].position, 0.2f);
            if (i < railPoints.Length - 1 && railPoints[i+1] != null)
                Gizmos.DrawLine(railPoints[i].position, railPoints[i+1].position);
        }

        // Visualisation de la détection
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleDetectionRange);
    }
#endif
}