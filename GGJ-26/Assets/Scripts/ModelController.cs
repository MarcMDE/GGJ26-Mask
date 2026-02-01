using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] private Material[] modelsMaterials;
    [SerializeField] private GameObject maskModel;

    int currentFaction = 0;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public int GetFaction()
    {
        return currentFaction;
    }

    public void SetFaction(int setFaction)
    {
        if (setFaction >= modelsMaterials.Length) return;

        currentFaction = setFaction;
        maskModel.GetComponent<Renderer>().material = modelsMaterials[currentFaction];
        
    }
}
