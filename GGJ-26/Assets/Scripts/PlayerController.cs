using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isEmoteActive;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Estos métodos son llamados por el componente Player Input
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnAction(InputValue value) => isEmoteActive = true;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleEmote();
    }

    void HandleMovement()
    {
        // Movimiento en el plano X y Z
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        
    }

    void HandleEmote()
    {
        if (isEmoteActive)
        {
            Debug.Log("Emote active");
            isEmoteActive = false;
        }
    }
}
