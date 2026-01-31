using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

// We swap CharacterController for Rigidbody and NavMeshAgent
[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 15f;
    public float rotationSpeed = 10f;

    [Header("Crowd Physics")]
    public float pushForce = 7.0f;
    public float collisionSlowdown = 0.5f;
    public float recoveryRate = 2.0f;

    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private Vector2 moveInput;
    private bool isEmoteActive;
    private float currentSpeedModifier = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        // Setup Rigidbody for physical interactions
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 5f; // Friction so you don't slide forever
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Setup NavMeshAgent so AI knows where the player is
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnAction(InputValue value) => isEmoteActive = true;

    void Update()
    {
        // Keep the NavMesh "ghost" synced to our physical body
        navAgent.nextPosition = transform.position;

        // Recover speed smoothly after leaving a collision
        currentSpeedModifier = Mathf.MoveTowards(currentSpeedModifier, 1.0f, Time.deltaTime * recoveryRate);

        HandleRotation();
        HandleEmote();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Calculate direction based on input
        Vector3 targetDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed * currentSpeedModifier;

        // Apply force to reach target velocity (Physical movement)
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0; // Don't interfere with gravity

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    void HandleRotation()
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion targetRot = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed));
        }
    }

    void HandleEmote()
    {
        if (isEmoteActive)
        {
            Debug.Log("Emote active");
            isEmoteActive = false;
        }
    }

    // This makes the player interact with the crowd agents
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
        {
            currentSpeedModifier = collisionSlowdown;

            Vector3 pushDir = transform.position - collision.transform.position;
            pushDir.y = 0;
            pushDir.Normalize();

            // Push self away
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);

            // Push the agent away (Mutual force)
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody agentRb))
            {
                agentRb.AddForce(-pushDir * (pushForce * 0.5f), ForceMode.Acceleration);
            }
        }
    }
}