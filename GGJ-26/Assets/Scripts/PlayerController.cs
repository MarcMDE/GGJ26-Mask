using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isGamepad;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Estos métodos son llamados por el componente Player Input
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Movimiento en el plano X y Z
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        // Detectar si el último input fue de mando o ratón
        var playerInput = GetComponent<PlayerInput>();
        isGamepad = playerInput.currentControlScheme == "Gamepad";

        if (isGamepad)
        {
            if (lookInput.sqrMagnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            }
        }
        else
        {
            // Rotación hacia el ratón (Raycast al suelo)
            Ray ray = Camera.main.ScreenPointToRay(lookInput);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPoint = hit.point;
                targetPoint.y = transform.position.y; // Mantener la altura del personaje
                transform.LookAt(targetPoint);
            }
        }
    }
}
