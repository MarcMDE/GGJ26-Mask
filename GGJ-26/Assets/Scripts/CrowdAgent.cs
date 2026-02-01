using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class CrowdAgent : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 5f;

    [Header("Conga-Line Breaker")]
    [Range(0, 1)] public float deviationProbability = 0.4f;
    public float deviationRadius = 3.0f;
    public float minCheckInterval = 1.0f; // Lower bound for the clock
    public float maxCheckInterval = 4.0f; // Upper bound for the clock

    [Header("Randomized Speed")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 4.5f;
    public float minAcceleration = 8f;
    public float maxAcceleration = 20f;

    [Header("Advanced Physics")]
    public float pushForce = 5.0f;
    public float collisionSlowdown = 0.4f;
    public float recoveryRate = 2.0f;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isChoosingDestination = false;
    private Bounds wanderBounds;
    private bool boundsSet = false;

    private float currentSpeedModifier = 1.0f;
    private float baseTargetSpeed;
    private Vector3 currentMoveTarget;
    private float myCheckInterval; // Per-agent unique interval

    [Header("Animator")]
    private Animator animator;

    private CircleConstraint circleConstraint;  

    public void InitializeBounds(Bounds bounds)
    {
        wanderBounds = bounds;
        boundsSet = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Set a unique rhythm for this specific agent
        myCheckInterval = Random.Range(minCheckInterval, maxCheckInterval);

        agent.updatePosition = false;
        agent.updateRotation = true;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        agent.avoidancePriority = Random.Range(0, 100);
        ApplyRandomPhysics();

        StartCoroutine(PathDeviationRoutine());
    }

    void Update()
    {
        if (!boundsSet) return;
        if (animator.GetBool("isDead"))
        {
            StopNavigation();
            return;
        }

        currentSpeedModifier = Mathf.MoveTowards(currentSpeedModifier, 1.0f, Time.deltaTime * recoveryRate);
        agent.speed = baseTargetSpeed * currentSpeedModifier;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isChoosingDestination) StartCoroutine(ChooseNewDestination());
        }
    }

    IEnumerator PathDeviationRoutine()
    {
        while (true)
        {
            // Wait for THIS agent's specific interval
            yield return new WaitForSeconds(myCheckInterval);

            if (boundsSet && !isChoosingDestination && agent.hasPath && circleConstraint == null)
            {
                if (Random.value < deviationProbability)
                {
                    Vector2 randomCircle = Random.insideUnitCircle * deviationRadius;
                    Vector3 deviation = new Vector3(randomCircle.x, 0, randomCircle.y);

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(currentMoveTarget + deviation, out hit, deviationRadius * 1.5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        if (worldDeltaPosition.magnitude > agent.radius)
        {
            agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
        }

        Vector3 velocity = worldDeltaPosition / Time.fixedDeltaTime;
        velocity.y = rb.linearVelocity.y;
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
        currentMoveTarget = GetRandomPoint();
        agent.SetDestination(currentMoveTarget);

        isChoosingDestination = false;
    }

    Vector3 GetRandomPoint()
    {
        Vector3 randomPoint;
        if (circleConstraint!= null)
        {
            var unit = Random.insideUnitCircle * circleConstraint.radius;
            randomPoint = circleConstraint.center + new Vector3(unit.x, wanderBounds.center.y, unit.y);
            return randomPoint;
        }
        

        float rx = Random.Range(wanderBounds.min.x, wanderBounds.max.x);
        float rz = Random.Range(wanderBounds.min.z, wanderBounds.max.z);
        randomPoint = new Vector3(rx, wanderBounds.center.y, rz);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    public void StopNavigation()
    {
        agent.isStopped = true;
        this.enabled = false;
    }

    public void SetCircleConstraint(Vector3 center, float radius)
    {
        circleConstraint = new CircleConstraint(center, radius);
        agent.SetDestination(transform.position);
    }



    public void ResetCircleConstraint()
    {
        circleConstraint = null;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            pushDir.y = 0;

            currentSpeedModifier = collisionSlowdown;
            rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);

            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody agentRb))
            {
                agentRb.AddForce(-pushDir * (pushForce * 0.5f), ForceMode.Acceleration);
            }
        }
    }

    
}
public class CircleConstraint
{
    public float radius;
    public Vector3 center;

    public CircleConstraint(Vector3 center,float radius)
    {
        this.radius = radius;
        this.center = center;
    }
}