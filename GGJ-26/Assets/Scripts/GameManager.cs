using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] PlayerSpawnHandler playerSpawnHandler;
    [SerializeField] PlayersTracker playersTracker;
    [SerializeField] CrowdSpawner crowdSpawner;
    [SerializeField] TextMeshProUGUI winText;
    
    void Start()
    {
        playerInputManager.DisableJoining();
        playerSpawnHandler.enabled = false;

        StartCoroutine(GameFlowCoroutine());
    }

    IEnumerator GameFlowCoroutine()
    {
        Debug.Log("Game started");
        yield return StartAgentsSpawningCoroutine();
        yield return WaitForPlayersCoroutine();
        yield return PlayGameCoroutine();
        yield return GameOverCoroutine();
        //yield return new WaitUntil(() => CheckGameOver());
    }

    IEnumerator StartAgentsSpawningCoroutine()
    {
        Debug.Log("Spawning crowd agents...");
        // TODO: Crowd spawner spawn
        // TODO: Wait for agents to move into the scene
        yield return null;
    }

    IEnumerator WaitForPlayersCoroutine()
    {
        Debug.Log("Waiting for players to join...");
        // TODO: Show UI to ask for players
        playerSpawnHandler.enabled = true;
        playerInputManager.EnableJoining();
        yield return new WaitUntil(() => playersTracker.NumPlayersConnected > 1);
        // TODO: Show UI indicating that joining is about to end
        Debug.Log("There are enough players, starting game in 10 seconds...");
        yield return new WaitForSeconds(10f); // Wait additional time for more players to join
        playerInputManager.DisableJoining();
        playerSpawnHandler.enabled = false;
        // TODO: Disable player joining UI
    }

    IEnumerator PlayGameCoroutine()
    {
        Debug.Log("Play game!");
        // TODO: Show controls + explain game rules
        yield return new WaitForSeconds(3f);
        // TODO: 3, 2, 1 Go! countdown
        yield return new WaitUntil(() => playersTracker.NumPlayersAlive <= 1 || playersTracker.NumPlayersConnected <= 1);

        var player = playersTracker.GetLastPlayerAlive();

        if (player != null)
        {
            winText.text = "WINNER!";

            player.transform.localScale = new Vector3(5, 5, 5);
        }
        yield return new WaitForSeconds(8);

        SceneManager.LoadScene("MenuScene");
    }

    IEnumerator GameOverCoroutine()
    {
        Debug.Log("Game Over!");
        // Find winner (if any)
        // TODO: Show game over UI
        yield return new WaitForSeconds(5f);
        // TODO: Reset game state
    }
}
