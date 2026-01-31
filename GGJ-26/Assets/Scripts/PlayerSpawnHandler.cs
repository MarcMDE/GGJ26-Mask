using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnHandler : MonoBehaviour
{
    List<CrowdAgent> crowdAgents;
    private int playerIndex = 0;

    void OnEnable()
    {
        crowdAgents = FindAnyObjectByType<CrowdSpawner>().GetAgents();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.position = crowdAgents[playerIndex % crowdAgents.Count].transform.position;
        playerIndex++;
    }
}
