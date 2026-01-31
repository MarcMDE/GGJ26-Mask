using System.Collections;
using UnityEngine;

public class DieController : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        animator.SetTrigger("DieTrigger");
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        animator.SetBool("isDead", true);
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        gameObject.SetActive(false);
    }
}
