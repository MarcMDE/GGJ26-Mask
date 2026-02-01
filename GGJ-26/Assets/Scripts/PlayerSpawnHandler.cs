using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerSpawnHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent<PlayerInput> OnPlayerSpawned;
    List<CrowdAgent> crowdAgents;
    CrowdSpawner crowdSpawner;
    private int spawnedPlayersCounter;
    int nCrowdAgents;

    void OnEnable()
    {
        crowdSpawner = FindAnyObjectByType<CrowdSpawner>();
        crowdAgents = crowdSpawner.GetAgents();
        nCrowdAgents = crowdAgents.Count;
        spawnedPlayersCounter = 0;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (spawnedPlayersCounter >= nCrowdAgents)
        {
            Debug.LogError("No crowd agents available for player spawn!");
            return;
        }

        var agent = crowdAgents.First();
        spawnedPlayersCounter++;
        playerInput.transform.position = agent.transform.position;
        playerInput.transform.rotation = agent.transform.rotation;
        playerInput.GetComponent<ModelController>().SetFaction(
            agent.GetComponent<ModelController>().GetFaction()
        );
        crowdSpawner.RemoveAgent(agent.gameObject);
        OnPlayerSpawned?.Invoke(playerInput);
    }
}