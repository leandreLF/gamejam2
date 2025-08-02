using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class RailMover : MonoBehaviour
{
    public static event UnityAction<bool> OnFreezeStateChanged;
    public static event UnityAction OnGameStarted;

    [Header("Rail Settings")]
    [SerializeField] private Transform[] railPoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waypointThreshold = 0.1f;

    [Header("UI Settings")]
    [SerializeField] private string readyUIContainerName = "ReadyUIContainer";
    [SerializeField] private string readyButtonName = "ReadyButton";
    private GameObject readyUIContainer;
    private Button readyButton;

    [Header("Combat Settings")]
    [SerializeField] private float kickDamage = 20f;
    [SerializeField] private float kickCooldown = 1f;
    [SerializeField] private float obstacleDetectionRange = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Health health;
    private Animator animator;
    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool canKick = true;

    public bool IsFrozen { get; private set; } = true;

    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();

        FindUIElements();
        health.OnDeath += OnPlayerDeath;
    }

    private void FindUIElements()
    {
        // Recherche dynamique des éléments UI
        if (readyUIContainer == null)
        {
            readyUIContainer = GameObject.Find(readyUIContainerName);
        }

        if (readyButton == null)
        {
            GameObject buttonObj = GameObject.Find(readyButtonName);
            if (buttonObj != null) readyButton = buttonObj.GetComponent<Button>();
        }

        if (readyButton != null && readyUIContainer != null)
        {
            readyButton.onClick.AddListener(OnReadyPressed);
        }
        else
        {
            Debug.LogWarning("UI elements not found dynamically. Game may not function properly.");
        }
    }

    void Update()
    {
        bool shouldBeMoving = !IsFrozen && currentPointIndex < railPoints.Length;
        if (isMoving != shouldBeMoving)
        {
            isMoving = shouldBeMoving;
            UpdateAnimator();
        }

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
            if (currentPointIndex >= railPoints.Length)
            {
                isMoving = false;
                UpdateAnimator();
            }
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
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ResetCurrentRoom();
        }
        else
        {
            Debug.LogWarning("RoomManager instance not found!");
        }
        if (health != null && health.isDead)
        {
            health.ResetHealth();
        }

        if (readyUIContainer != null)
        {
            readyUIContainer.SetActive(false);
        }

        StartMovement();
    }

    public void StartMovement()
    {
        IsFrozen = false;
        isMoving = true;
        OnFreezeStateChanged?.Invoke(false);
        OnGameStarted?.Invoke();
        UpdateAnimator();
    }

    void OnPlayerDeath()
    {
        IsFrozen = true;
        isMoving = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

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

        bool shouldAnimateMove = isMoving && !IsFrozen;
        animator.SetBool("isMoving", shouldAnimateMove);
    }

    void OnDestroy()
    {
        health.OnDeath -= OnPlayerDeath;
        if (readyButton != null)
            readyButton.onClick.RemoveListener(OnReadyPressed);
    }

    public void ResetPlayer()
    {
        IsFrozen = true;
        isMoving = false;
        currentPointIndex = 0;

        if (railPoints.Length > 0 && railPoints[0] != null)
        {
            transform.position = railPoints[0].position;
        }

        FindUIElements();

        if (readyUIContainer != null)
        {
            readyUIContainer.SetActive(true);
        }

        if (readyButton != null)
        {
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyPressed);
        }

        UpdateAnimator();

        if (health != null && health.isDead)
        {
            health.ResetHealth();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < railPoints.Length; i++)
        {
            if (railPoints[i] == null) continue;
            Gizmos.DrawSphere(railPoints[i].position, 0.2f);
            if (i < railPoints.Length - 1 && railPoints[i+1] != null)
                Gizmos.DrawLine(railPoints[i].position, railPoints[i+1].position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleDetectionRange);
    }
#endif
}