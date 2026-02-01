using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayersTracker : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    public event UnityAction OnNumPlayersConnectedChanged;
    public event UnityAction OnNumPlayersAliveChanged;

    public int NumPlayersAlive { get; private set; }
    public int NumPlayersConnected { get; private set; }

    void Start()
    {
        NumPlayersConnected = 0;
        NumPlayersAlive = 0;
    }

    public GameObject GetLastPlayerAlive()
    {
            return players
            .Select(p => p.gameObject)
            .FirstOrDefault(p => !p.GetComponentInChildren<Animator>().GetBool("isDead"));
    }

    private void OnPlayerKilled(GameObject player)
    {
        NumPlayersAlive--;
        Destroy(player);
        OnNumPlayersAliveChanged?.Invoke();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        NumPlayersConnected++;
        NumPlayersAlive++;
        
        players.Add(player);
        player.GetComponent<DieController>().SetDieDelegate(OnPlayerKilled);
        OnNumPlayersConnectedChanged?.Invoke();
    }

    public void OnPlayerDisconnected(PlayerInput player)
    {
        Debug.Log($"Player {player.name} disconected.");
        NumPlayersConnected--;
        OnNumPlayersConnectedChanged?.Invoke();
        //players.Remove(player);
    }
}