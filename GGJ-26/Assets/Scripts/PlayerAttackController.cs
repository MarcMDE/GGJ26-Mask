using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private float attackCoolDown = 3f;
        [SerializeField] private float attackConeDotThreshold = 0.6f;
        private Animator animator;
        bool isAttackAvailable;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            isAttackAvailable = true;
        }

        public void Attack()
        {
            StartCoroutine(AttackCR());
            // TODO: Logica atac
        }

        IEnumerator AttackCR()
        {
            if (!isAttackAvailable) yield break;

            isAttackAvailable = false;
            animator.SetBool("AttackQueued", true);


            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") )
            {
                if (!animator.GetBool("AttackQueued"))
                {
                    isAttackAvailable = true;
                    yield break;
                }
                yield return null;
            }

            TryToKillCharacter();

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));


            yield return AttackCoolDownCoroutine(attackCoolDown);
        }

        IEnumerator AttackCoolDownCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            isAttackAvailable = true;
        }

        private void TryToKillCharacter()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Character"));

            GameObject nearestCharacter = null;
            float nearestValue = -1f;

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == this.gameObject) continue;

                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

                // 3. Dot Product entre mi frente (Forward) y la dirección al objeto
                // 1 = exactamente enfrente, 0 = perpendicular, -1 = exactamente detrás
                float dot = Vector3.Dot(transform.forward, directionToTarget);

                // 4. Seleccionar el que tenga el valor más cercano a 1
                if (dot > attackConeDotThreshold && dot > nearestValue)
                {
                    nearestValue = dot;
                    nearestCharacter = hit.gameObject;
                }
            }

            if (nearestCharacter != null)
            {
                nearestCharacter.GetComponent<DieController>().Die();
            }
        }
    }
}