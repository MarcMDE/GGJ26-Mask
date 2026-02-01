using System;
using System.Collections;
using UnityEngine;

public class DieController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] GameObject maskModel;
    [SerializeField] GameObject characterModel;

    private Action<GameObject> onDeath;

    private bool dead;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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
        maskModel.SetActive(false);
        animator.SetBool("isDead", true);

        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        characterModel.transform.parent = null;
        onDeath?.Invoke(gameObject);
    }
}
