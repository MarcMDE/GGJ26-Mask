using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameStates
{
    Loading,
    SpawningCrowd,
    WaitingForPlayers,
    FindYourself,
    Playing,
    GameOver,
    Pause
}

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] PlayerSpawnHandler playerSpawnHandler;
    [SerializeField] PlayersTracker playersTracker;
    [SerializeField] CrowdSpawner crowdSpawner;

    [SerializeField] EmoteTimer emoteTimer;
    [SerializeField] AudioSource music;

    [SerializeField] TextMeshProUGUI uiTitleText;
    [SerializeField] TextMeshProUGUI uiDescText;
    [SerializeField] TextMeshProUGUI uiDesc2Text;
    [SerializeField] UnityEngine.UI.Image uiControlsImage;

    [Header("Flow settings")]
    [SerializeField] private int waitForPlayersSeconds = 3;
    [SerializeField] private float loadingTime = 1f;
 
    public static GameStates CurrentGameState { get; private set; }

    void Start()
    {
        CurrentGameState = GameStates.Loading;
        uiControlsImage.gameObject.SetActive(true);
        playerInputManager.DisableJoining();
        playerSpawnHandler.enabled = false;

        uiDescText.text = "";
        uiDesc2Text.text = "";
        uiTitleText.text = "Loading...";

        StartCoroutine(GameFlowCoroutine());
    }

    private void NumPlayersConnectedChanged()
    {
        uiDesc2Text.text = $"There are {playersTracker.NumPlayersConnected} players connected";
    }

    private void NumPlayersAliveChanged()
    {   if (playersTracker.NumPlayersAlive <= 1)
        {
            uiDesc2Text.text = $"Last player standing!";
            CurrentGameState = GameStates.GameOver;
        }
        else
            uiDesc2Text.text = $"{playersTracker.NumPlayersAlive} players alive";
    }

    IEnumerator GameFlowCoroutine()
    {
        Debug.Log("Game started");
        yield return StartAgentsSpawningCoroutine();
        yield return WaitForPlayersCoroutine();
        yield return PlayGameCoroutine();
        yield return GameOverCoroutine();
    }

    IEnumerator StartAgentsSpawningCoroutine()
    {
        CurrentGameState = GameStates.SpawningCrowd;
        Debug.Log("Spawning crowd agents...");
        crowdSpawner.SpawnCrowd();
        // TODO: Sync wait for agents to be spawned
        yield return new WaitForSeconds(loadingTime);
        uiControlsImage.gameObject.SetActive(false);

        music.Play();
    }

    IEnumerator WaitForPlayersCoroutine()
    {
        CurrentGameState = GameStates.WaitingForPlayers;
        uiTitleText.text = "Press ANY buttton to JOIN THE GAME!";
        uiDescText.text = "At least 2 players are required to start";
        // TODO: Show UI to ask for players
        playersTracker.OnNumPlayersConnectedChanged += NumPlayersConnectedChanged;
        playerSpawnHandler.enabled = true;
        playerInputManager.EnableJoining();
        yield return new WaitUntil(() => playersTracker.NumPlayersConnected > 1);
        // TODO: Show UI indicating that joining is about to end

        music.Stop();

        uiDescText.text = $"Game will start in {waitForPlayersSeconds} seconds!";
        yield return new WaitForSeconds(1.5f);

        for (int i=(int)waitForPlayersSeconds-1; i>0; i--)
        {
            uiDescText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }

        playerInputManager.DisableJoining();
        playerSpawnHandler.enabled = false;
        playersTracker.OnNumPlayersConnectedChanged -= NumPlayersConnectedChanged;
        uiDesc2Text.text = "";
        uiDescText.text = "";
    }

    IEnumerator PlayGameCoroutine()
    {
        CurrentGameState = GameStates.FindYourself;
        uiTitleText.text = "START!";
        yield return new WaitForSeconds(1f);
        uiTitleText.text = "Find yourself!";
        yield return new WaitForSeconds(3f);
        uiTitleText.text = "UNMASK the other players TO WIN!";
        CurrentGameState = GameStates.Playing;
        playersTracker.OnNumPlayersAliveChanged += NumPlayersAliveChanged;
        yield return new WaitForSeconds(3f);
        uiTitleText.text = "";

        emoteTimer.StartLoop();
        music.Play();
        

        //yield return new WaitUntil(() => playersTracker.NumPlayersAlive <= 1 || playersTracker.NumPlayersConnected <= 1);
        yield return new WaitUntil(() => CurrentGameState == GameStates.GameOver);
        playersTracker.OnNumPlayersAliveChanged -= NumPlayersAliveChanged;
    }

    IEnumerator GameOverCoroutine()
    {
        var player = playersTracker.GetLastPlayerAlive();

        if (player != null)
        {
            uiTitleText.text = "WINNER!";
            player.transform.localScale = new Vector3(5, 5, 5);
        }

        yield return new WaitForSeconds(8);

        SceneManager.LoadScene("MenuScene");
    }
}
