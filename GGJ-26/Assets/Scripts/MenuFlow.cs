
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFlow : MonoBehaviour
{
    [SerializeField] private string gameScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
