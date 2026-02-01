using System;
using System.Collections;
using UnityEngine;

public class DieController : MonoBehaviour
{
    private Animator animator;

    private Action<GameObject> onDeath;

    private bool dead;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        dead = true;
        animator.SetTrigger("DieTrigger");
        StartCoroutine(DieCoroutine());
    }

    public void SetDieDelegate(Action<GameObject> dieFunc) {
        onDeath = dieFunc;
    }

    public bool IsDead()
    {
        return dead;
    }


    private IEnumerator DieCoroutine()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        animator.SetBool("isDead", true);
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));

       onDeath?.Invoke(gameObject);
    }
}
