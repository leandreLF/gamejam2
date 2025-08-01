using UnityEngine;
using UnityEngine.InputSystem;

public class Cam : MonoBehaviour
{
    private CameraMovements controls;  // Classe auto-g�n�r�e issue de Cam.inputactions
    private Vector2 moveInput;

    private void Awake()
    {
        controls = new CameraMovements();

        // Acc�s � l�Action Map "Camera" puis � l�action "Move"
        controls.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Camera.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();  // Active le syst�me Input
    }

    private void OnDisable()
    {
        controls.Disable();  // D�sactive le syst�me Input
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        transform.position += movement * Time.deltaTime * 5f;  // Ajuste la vitesse ici
    }
}
