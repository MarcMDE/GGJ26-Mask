using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isEmoteActive;
    private bool isAttackActive;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Estos métodos son llamados por el componente Player Input
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnEmote(InputValue value) => isEmoteActive = true;
    public void OnAttack(InputValue value) => isAttackActive = true;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleEmote();
        HandleAttack();
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
            Debug.Log($"{gameObject.name} Emote");
            isEmoteActive = false;
        }
    }

    void HandleAttack()
    {
        if (isAttackActive)
        {
            Debug.Log($"{gameObject.name} Attack");
            isAttackActive = false;
        }
    }
}
