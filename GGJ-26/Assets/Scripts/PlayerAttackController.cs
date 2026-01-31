using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class PlayerAttackController : PlayerControllerBase
    {
        [SerializeField] private float attackCoolDown = 3f;
        [SerializeField] private float attackDuration = 1.5f;
        private Animator animator;
        private bool isAttackAvailable;
        public bool IsAttacking { get; private set; }

        new void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            isAttackAvailable = true;
            IsAttacking = false;
        }

        public void Attack()
        {
            if (!isAttackAvailable) return;
            if (state.TrySetState(States.ATTACK) == false) return;

            animator.SetTrigger("AttackTrigger");
            Debug.Log($"{gameObject.name} Attack");
            isAttackAvailable = false;
            IsAttacking = true;
            StartCoroutine("AttackDurationCoroutine", attackDuration);
            StartCoroutine("AttackCoolDownCoroutine", attackCoolDown);
        }

        IEnumerator AttackCoolDownCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            isAttackAvailable = true;
        }

        IEnumerator AttackDurationCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            IsAttacking = false;
            state.TrySetState(States.NONE);
        }
    }
}