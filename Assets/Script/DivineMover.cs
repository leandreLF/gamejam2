using UnityEngine;
using UnityEngine.InputSystem;

public class DivineGrabMouse : MonoBehaviour
{
    [Header("Paramètres de saisie")]
    public float grabDistance = 10f;
    public float moveSpeed = 20f;
    public LayerMask grabbableLayer;

    private Camera cam;
    private Rigidbody grabbedObject;

    private PlayerControls controls;
    private InputAction grabAction;

    private Plane grabPlane;

    void Awake()
    {
        cam = Camera.main;
        controls = new PlayerControls();
        grabAction = controls.Player.Grab;

        grabAction.performed += ctx => TryGrabAtCursor();
        grabAction.canceled += ctx => ReleaseObject();

        // Plan horizontal à hauteur 0 (ajuste si besoin)
        grabPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (grabbedObject != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (grabPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = hitPoint - grabbedObject.position;
                grabbedObject.linearVelocity = direction * moveSpeed;
            }
        }
    }

    private void TryGrabAtCursor()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
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

    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.useGravity = true;
            grabbedObject.linearDamping = 0f;
            grabbedObject.linearVelocity = Vector3.zero;
            grabbedObject.angularVelocity = Vector3.zero;
            grabbedObject = null;
            Debug.Log("Objet lâché");
        }
    }
}
