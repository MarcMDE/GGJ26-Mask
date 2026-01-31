using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class PhysicalPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;

    [Header("Crowd Interaction")]
    public float pushForce = 5.0f;
    public float collisionSlowdown = 0.5f;

    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private Vector3 moveInput;
    private float currentSpeedModifier = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        // 1. Setup Rigidbody for physical crowding
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 5f; // Matches agents so you don't "slide" too much
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // 2. Setup NavMeshAgent so AI treats you as a peer
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;
    }

    void Update()
    {
        // Capture WASD / Arrow keys
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0, z).normalized;

        // Sync the NavMeshAgent "ghost" to our physical position
        // This ensures the AI knows exactly where we are for their avoidance math
        navAgent.nextPosition = transform.position;

        // Smoothly recover from bumps
        currentSpeedModifier = Mathf.MoveTowards(currentSpeedModifier, 1.0f, Time.deltaTime * 2f);
    }

    void FixedUpdate()
    {
        // 3. Move via Velocity instead of Transform
        // This is the secret to "bumping" smoothly
        Vector3 targetVelocity = moveInput * moveSpeed * currentSpeedModifier;

        // Calculate the force needed to reach target velocity
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0; // Don't mess with gravity

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        // Rotation
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }

    // Identical collision logic to the agents
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agent"))
        {
            Vector3 pushDir = transform.position - collision.transform.position;
            pushDir.y = 0;
            pushDir.Normalize();

            // Both the player and the agent feel the "struggle"
            currentSpeedModifier = collisionSlowdown;
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);
        }
    }
}