using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
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

    [Header("Crowd Physics")]
    public float pushStrength = 0.05f;

    private NavMeshAgent agent;
    private bool isChoosingDestination = false;
    private Bounds wanderBounds;
    private bool boundsSet = false;

    // The Spawner will call this right after Instantiate
    public void InitializeBounds(Bounds bounds)
    {
        wanderBounds = bounds;
        boundsSet = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);
        ApplyRandomPhysics();
    }

    void Update()
    {
        if (!boundsSet) return; // Wait until we know where the walls are

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isChoosingDestination)
            {
                StartCoroutine(ChooseNewDestination());
            }
        }
    }

    void ApplyRandomPhysics()
    {
        agent.speed = Random.Range(minSpeed, maxSpeed);
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
        // 1. Pick random X and Z within the bounds handed down by the spawner
        float rx = Random.Range(wanderBounds.min.x, wanderBounds.max.x);
        float rz = Random.Range(wanderBounds.min.z, wanderBounds.max.z);

        Vector3 randomPoint = new Vector3(rx, wanderBounds.center.y, rz);

        NavMeshHit hit;
        // 2. Snap it to the NavMesh
        if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // Fallback
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = transform.position - collision.transform.position;
            pushDir.y = 0;
            transform.position += pushDir.normalized * pushStrength;
        }
    }
}