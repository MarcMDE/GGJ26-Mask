using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] private GameObject[] models;

    int currentFaction = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetFaction()
    {
        return currentFaction;
    }

    public void SetFaction(int setFaction)
    {
        if (setFaction >= models.Length) return;

        currentFaction = setFaction;

        for (int i = 0; i < models.Length; i++) {
            models[i].SetActive(currentFaction == i);
        }
    }
}
