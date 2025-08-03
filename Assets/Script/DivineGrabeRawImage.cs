using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DivineGrabFromRawImage : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private RawImage rawImage;     // Type RawImage, pas GameObject
    [SerializeField] private Camera pixelCamera;    // Type Camera, pas GameObject
    [SerializeField] private LayerMask grabbableLayer;

    [Header("Grab Settings")]
    public float grabDistance = 10f;
    public float moveSpeed = 20f;

    private Rigidbody grabbedObject;
    private PlayerControls controls;
    private InputAction grabAction;
    private Plane grabPlane;

    void Awake()
    {
        if (rawImage == null)
            rawImage = GameObject.Find("RawImage").GetComponent<RawImage>();

        if (pixelCamera == null)
            pixelCamera = GameObject.Find("pixelCamera").GetComponent<Camera>();
        controls = new PlayerControls();
        grabAction = controls.Player.Grab;

        grabAction.performed += ctx => TryGrabFromRawImage();
        grabAction.canceled += ctx => ReleaseObject();

        grabPlane = new Plane(Vector3.up, Vector3.zero); // plan horizontal à y = 0
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (grabbedObject != null)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImage.rectTransform,
                Mouse.current.position.ReadValue(),
                null,
                out localCursor
            );

            Rect rect = rawImage.rectTransform.rect;
            float normX = (localCursor.x - rect.x) / rect.width;
            float normY = (localCursor.y - rect.y) / rect.height;
            Vector3 viewportPoint = new Vector3(normX, normY, 0f);

            Ray ray = pixelCamera.ViewportPointToRay(viewportPoint);

            if (grabPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = hitPoint - grabbedObject.position;
                grabbedObject.linearVelocity = direction * moveSpeed;
            }
        }
    }

    void TryGrabFromRawImage()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Le clic est sur l'UI, ici tu peux décider si tu veux ignorer ou pas
        }

        Vector2 localCursor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            Mouse.current.position.ReadValue(),
            null,
            out localCursor
        );

        Rect rect = rawImage.rectTransform.rect;
        float normX = (localCursor.x - rect.x) / rect.width;
        float normY = (localCursor.y - rect.y) / rect.height;

        Vector3 viewportPoint = new Vector3(normX, normY, 0f);
        Ray ray = pixelCamera.ViewportPointToRay(viewportPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, grabbableLayer))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                if (grabbedObject != null)
                    ReleaseObject();

                grabbedObject = rb;
                grabbedObject.useGravity = false;
                grabbedObject.linearDamping = 10f;
                Debug.Log("Objet saisi : " + grabbedObject.name);
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.useGravity = true;
            grabbedObject.linearDamping = 0f;
            grabbedObject.linearVelocity = Vector3.zero;
            grabbedObject.angularVelocity = Vector3.zero;

            var resettable = grabbedObject.GetComponent<ResettableObject>();
            if (resettable != null)
            {
                resettable.UpdateInitialStateToCurrent();
            }

            grabbedObject = null;
            Debug.Log("Objet lâché");
        }
    }
}
