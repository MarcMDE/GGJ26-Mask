using UnityEngine;

public class InteractionController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            Debug.Log("Enemy hit!");
        }
    }
}
