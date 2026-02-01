using System.Collections;
using Mask.Player;
using UnityEngine;

public class EmoteTimer : MonoBehaviour
{
    [SerializeField] private float emotePeriod = 5;

    private CrowdSpawner crowdSpawner;

    private void Start()
    {
        crowdSpawner = GetComponent<CrowdSpawner>();
        
    }
    
    public void StartLoop()
    {
        StartCoroutine(AgentsEmote());
    }

    IEnumerator AgentsEmote()
    {
       // var agents
        while (true)
        {
            yield return new WaitForSeconds(emotePeriod);

            var agents = crowdSpawner.GetAgents();

            if (agents != null)
            {
                foreach (var agent in agents)
                {
                    if(agent.isActiveAndEnabled) agent.GetComponent<EmoteController>().EmoteDelayed();
                }
            }
        }
    }
}
