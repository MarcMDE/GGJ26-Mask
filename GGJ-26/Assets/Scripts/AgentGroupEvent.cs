using UnityEngine;

public class AgentGroupEvent : MonoBehaviour
{
    [SerializeField] private float groupPeriodMin = 5;
    [SerializeField] private float groupPeriodMax = 20;

    private CrowdSpawner crowdSpawner;
    private float nextEventCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetEventTimer();
    }

    private void ResetEventTimer()
    {
        nextEventCounter = Random.Range(groupPeriodMin, groupPeriodMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextEventCounter > 0) nextEventCounter -= Time.deltaTime;
        else TriggerEvent();
    }

    private void TriggerEvent()
    {
        


        //var points = NavMeshUtils.GetMultipleSafePoints();
    }

}
