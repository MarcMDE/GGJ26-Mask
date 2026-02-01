using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusUiElement : MonoBehaviour
{
    private GameObject previousSelectedObject; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Focus()
    {
        previousSelectedObject = EventSystem.current.currentSelectedGameObject;

        gameObject.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Unfocus()
    {
        gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(previousSelectedObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
