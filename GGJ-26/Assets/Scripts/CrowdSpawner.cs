using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Collections;
using Mask.Player; // Required for the new NavMeshSurface

public class CrowdSpawner : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int agentCount = 50;

    [SerializeField] private NavMeshSurface surface;

    [SerializeField] private int factionCount = 4;
    private Bounds spawnBounds;

    private List<CrowdAgent> agents = new List<CrowdAgent>();

    void Start()
    {
        CalculateBounds();
        //SpawnCrowd();
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

    public IEnumerator SpawnCrowd()
    {
        int spawned = 0;
        int attempts = 0;

        int offset = Random.Range(0, factionCount);

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
                var pos = new Vector3(hit.position.x, hit.position.y + 2, hit.position.z);
                
                GameObject newAgent = Instantiate(agentPrefab, pos, Quaternion.identity);
                newAgent.name = agentPrefab.name + spawned;
                newAgent.transform.parent = this.transform;

                // Hand the bounds to the agent
                if (newAgent.TryGetComponent<CrowdAgent>(out CrowdAgent crowdScript))
                {
                    newAgent.GetComponent<DieController>().SetDieDelegate(RemoveAgent);

                    int faction = (spawned + offset) % factionCount;

                    newAgent.GetComponent<ModelController>().SetFaction(faction);
                    crowdScript.InitializeBounds(spawnBounds);

                    agents.Add(crowdScript);
                }
                
                spawned++;

                yield return null; // Spread spawning over multiple frames
            }
        }
    }

    public void RemoveAgent(GameObject agent)
    {
        var crowdAgent = agent.GetComponent<CrowdAgent>();
        agents.Remove(crowdAgent);
        Destroy(agent);
    }

    public List<CrowdAgent> GetAgents()
    {
        return agents;
    }

    public int GetFactionCount()
    {
        return factionCount;
    }
}