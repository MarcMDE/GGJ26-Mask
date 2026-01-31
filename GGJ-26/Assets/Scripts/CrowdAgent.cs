using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class CrowdAgent : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 5f;

    [Header("Randomized Speed")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 4.5f;
    public float minAcceleration = 8f;
    public float maxAcceleration = 20f;

    [Header("Advanced Physics")]
    public float pushForce = 5.0f;        // Strength of the nudge
    public float collisionSlowdown = 0.4f; // Multiply speed by this during bumps (0.4 = 40% speed)
    public float recoveryRate = 2.0f;     // How fast they regain speed after a bump

    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isChoosingDestination = false;
    private Bounds wanderBounds;
    private bool boundsSet = false;

    private float currentSpeedModifier = 1.0f;
    private float baseTargetSpeed;

    public void InitializeBounds(Bounds bounds)
    {
        wanderBounds = bounds;
        boundsSet = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // 1. DISCONNECT the agent from the transform position
        // This stops the "fighting" that causes jitter
        agent.updatePosition = false;
        agent.updateRotation = true; // Keep this true so they face where they walk

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        agent.avoidancePriority = Random.Range(0, 100);
        ApplyRandomPhysics();
    }

    void Update()
    {
        if (!boundsSet) return;

        currentSpeedModifier = Mathf.MoveTowards(currentSpeedModifier, 1.0f, Time.deltaTime * recoveryRate);
        agent.speed = baseTargetSpeed * currentSpeedModifier;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isChoosingDestination) StartCoroutine(ChooseNewDestination());
        }
    }

    void FixedUpdate()
    {
        // 2. MANUALLY move the Rigidbody toward the Agent's internal position
        // This allows physics to "win" during a collision without jitter
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        if (worldDeltaPosition.magnitude > agent.radius)
        {
            // Smoothly pull the agent's internal ghost toward the physical body
            agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
        }

        // Apply the velocity to the Rigidbody
        Vector3 velocity = worldDeltaPosition / Time.fixedDeltaTime;
        velocity.y = rb.linearVelocity.y; // Preserve gravity
        rb.linearVelocity = Vector3.ClampMagnitude(velocity, agent.speed);
    }
    void ApplyRandomPhysics()
    {
        baseTargetSpeed = Random.Range(minSpeed, maxSpeed);
        agent.speed = baseTargetSpeed;
        agent.acceleration = Random.Range(minAcceleration, maxAcceleration);
    }

    IEnumerator ChooseNewDestination()
    {
        isChoosingDestination = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        ApplyRandomPhysics();

        Vector3 newPos = GetRandomPointInBounds();
        agent.SetDestination(newPos);

        isChoosingDestination = false;
    }

    Vector3 GetRandomPointInBounds()
    {
        float rx = Random.Range(wanderBounds.min.x, wanderBounds.max.x);
        float rz = Random.Range(wanderBounds.min.z, wanderBounds.max.z);
        Vector3 randomPoint = new Vector3(rx, wanderBounds.center.y, rz);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    // This handles the "Mutual" interaction
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
        {
            // Calculate direction from the other object to us
            Vector3 pushDir = transform.position - collision.transform.position;
            pushDir.y = 0;
            pushDir.Normalize();

            // Apply a "Stumble" speed penalty
            currentSpeedModifier = collisionSlowdown;

            // Apply physical nudge to self
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);

            // Apply slight push to the other agent (if it has a Rigidbody)
            // This creates the "Mutual Interaction" where both feel the bump
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody agentRb))
            {
                agentRb.AddForce(-pushDir * (pushForce * 0.5f), ForceMode.Acceleration);
            }
        }
    }
}