using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class PlayerEmoteController : PlayerControllerBase
    {
        [SerializeField] private float emoteDuration = 1.2f;
        private Animator animator;
        private bool isEmoteAvailable;

        new void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            isEmoteAvailable = true;
        }

        public void Emote()
        {
            if (state.TrySetState(States.EMOTE) == false) return;
            if (!isEmoteAvailable) return;

            animator.SetTrigger("EmoteTrigger");
            Debug.Log($"{gameObject.name} Emote");
            isEmoteAvailable = false;
            StartCoroutine("EmoteDurationCoroutine", emoteDuration);
        }

        IEnumerator EmoteDurationCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            isEmoteAvailable = true;
            state.TrySetState(States.NONE);
        }
    }
}
