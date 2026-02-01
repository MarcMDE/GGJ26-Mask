using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class PlayerAttackController : InteractionController
    {
        [SerializeField] private float attackCoolDown = 3f;

        private Animator animator;
        bool isAttackAvailable;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            isAttackAvailable = true;
        }

        public void Attack()
        {
            StartCoroutine(AttackCR());
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
            if (GameManager.CurrentGameState != GameStates.Playing) return;

            var nearestCharacter = GetClosestCharacter();

            if (nearestCharacter != null)
            {
                var die = GetComponent<DieController>();
                if (!die.IsDead()) nearestCharacter.GetComponent<DieController>().Die();
            }
        }
    }
}