using System.Collections;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;

public class AgentGroupEvent : MonoBehaviour
{
    [SerializeField] private float groupPeriodMin = 5;
    [SerializeField] private float groupPeriodMax = 20;

    [SerializeField] private NavMeshSurface surface;

    [SerializeField] private float minEventDuration = 6;
    [SerializeField] private float maxEventDuration = 10;

    private CrowdSpawner crowdSpawner;
    private float nextEventCounter;

    private Bounds bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crowdSpawner = GetComponent<CrowdSpawner>();
        if (surface.TryGetComponent<Collider>(out Collider collider))
        {
            bounds = collider.bounds;
        }
        
        StartCoroutine(AgentGroupCoroutine());
    }

    private void ResetEventTimer()
    {
        nextEventCounter = Random.Range(groupPeriodMin, groupPeriodMax);
    }

    private IEnumerator AgentGroupCoroutine()
    {
        ResetEventTimer();
        while (true)
        {
            if (nextEventCounter > 0)
            {
                nextEventCounter -= Time.deltaTime;

                yield return null;
                
            }
            else 
            {
                yield return TriggerEvent();
                ResetEventTimer(); 
            }
        }
    }

    private IEnumerator TriggerEvent()
    {
        if (bounds == null) yield break;

        var eventDuration = Random.Range(minEventDuration, maxEventDuration);

        var agents = crowdSpawner.GetAgents();

        var clusterCount = crowdSpawner.GetFactionCount();

        var agentsPerCluster = agents.Count / clusterCount;

        var radius = 1f;

        var points = NavMeshUtils.GetMultipleSafePoints(bounds, clusterCount, radius, 6, 800);

        foreach (var agent in agents) {
            var index = agent.GetComponent<ModelController>().GetFaction();
            agent.SetCircleConstraint(points[index], radius);
        
        }

        yield return new WaitForSeconds(eventDuration);

        agents = crowdSpawner.GetAgents();

        foreach (var agent in agents)
        {
            agent.ResetCircleConstraint();
        }
    }
}
