using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation; // Required for the new NavMeshSurface

public class BoundSpawner : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int agentCount = 50;

    [SerializeField] private NavMeshSurface surface;
    private Bounds spawnBounds;

    void Start()
    {
        CalculateBounds();
        SpawnCrowd();
    }

    void CalculateBounds()
    {
        // We get the renderer or collider bounds of the object
        // This assumes the script is on the floor/ground object
        if (surface.TryGetComponent<Renderer>(out Renderer renderer))
        {
            spawnBounds = renderer.bounds;
        }
        else if (surface.TryGetComponent<Collider>(out Collider collider))
        {
            spawnBounds = collider.bounds;
        }
    }

    void SpawnCrowd()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < agentCount && attempts < 1000)
        {
            attempts++;

            // Pick a random point inside the Bounds
            float rx = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            float rz = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

            // Use the center Y of the bounds to start the search
            Vector3 randomPoint = new Vector3(rx, spawnBounds.center.y, rz);

            NavMeshHit hit;
            // 5.0f range allows for height variance
            if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
            {
                GameObject newAgent = Instantiate(agentPrefab, hit.position, Quaternion.identity);
                newAgent.transform.parent = this.transform;

                // Hand the bounds to the agent
                if (newAgent.TryGetComponent<CrowdAgent>(out CrowdAgent crowdScript))
                {
                    crowdScript.InitializeBounds(spawnBounds);
                }

                spawned++;
            }
        }
    }

    // This shows the bounds in the editor so you can verify the area
    private void OnDrawGizmos()
    {
        if (spawnBounds != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(spawnBounds.center, spawnBounds.size);
        }
    }
}