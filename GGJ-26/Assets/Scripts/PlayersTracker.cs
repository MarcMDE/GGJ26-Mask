using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersTracker : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();

    public int NumPlayersAlive { get; private set; }
    public int NumPlayersConnected { get; private set; }

    void Start()
    {
        NumPlayersConnected = 0;
        NumPlayersAlive = 0;
    }

    public GameObject GetLastPlayerAlive()
    {
        if (NumPlayersAlive <= 1)
        {
            // TODO: Find last alive
        }
        
        return null;
    }

    private void OnPlayerKilled(GameObject player)
    {
        NumPlayersAlive--;
        Destroy(player);
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        NumPlayersConnected++;
        NumPlayersAlive++;
        
        players.Add(player);
        player.GetComponent<DieController>().SetDieDelegate(OnPlayerKilled);
    }

    public void OnPlayerDisconnected(PlayerInput player)
    {
        Debug.Log($"Player {player.name} disconected.");
        NumPlayersConnected--;
        //players.Remove(player);
    }
}