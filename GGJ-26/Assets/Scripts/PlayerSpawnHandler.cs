using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerSpawnHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent<PlayerInput> OnPlayerSpawned;
    List<CrowdAgent> crowdAgents;
    CrowdSpawner crowdSpawner;
    List<GameObject> usedCrowdAgents;
    private int playerIndex;
    int nCrowdAgents;

    void OnEnable()
    {
        crowdSpawner = FindAnyObjectByType<CrowdSpawner>();
        crowdAgents = crowdSpawner.GetAgents();
        nCrowdAgents = crowdAgents.Count;
        usedCrowdAgents = new List<GameObject>();
        playerIndex = 0;
    }

    void OnDisable()
    {
        OnSpawnPlayersEnd();
        usedCrowdAgents.Clear();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerIndex >= nCrowdAgents)
        {
            Debug.LogError("No crowd agents available for player spawn!");
            return;
        }

        var agent = crowdAgents[playerIndex];
        playerInput.transform.position = agent.transform.position;
        playerInput.transform.rotation = agent.transform.rotation;
        playerIndex++;
        usedCrowdAgents.Add(agent.gameObject);
        agent.gameObject.SetActive(false);
        OnPlayerSpawned?.Invoke(playerInput);
    }

    public void OnSpawnPlayersEnd()
    {
        foreach (var agent in usedCrowdAgents)
        {
            crowdSpawner.RemoveAgent(agent);
        }
    }
}