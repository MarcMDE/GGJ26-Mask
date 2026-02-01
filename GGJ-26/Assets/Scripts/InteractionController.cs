using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float attackConeDotThreshold = 0.5f;
    public GameObject GetClosestCharacter()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1.0f, LayerMask.GetMask("Character"));

        GameObject nearestCharacter = null;
        float nearestValue = -1f;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

            // 3. Dot Product entre mi frente (Forward) y la direcci�n al objeto
            // 1 = exactamente enfrente, 0 = perpendicular, -1 = exactamente detr�s
            float dot = Vector3.Dot(transform.forward, directionToTarget);

            // 4. Seleccionar el que tenga el valor m�s cercano a 1
            if (dot > attackConeDotThreshold && dot > nearestValue)
            {
                nearestValue = dot;
                nearestCharacter = hit.gameObject;
            }
        }
        return nearestCharacter;
    }
}
