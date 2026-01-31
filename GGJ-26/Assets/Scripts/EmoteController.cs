using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class EmoteController : MonoBehaviour
    {
        private Animator animator;

        [SerializeField] private float maxEmoteDelay = 0.2f;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {

        }

        public void Emote()
        {
            animator.SetBool("EmoteQueued", true);
        }

        public void EmoteDelayed()
        {
            StartCoroutine(EmoteDelayedCR());
        }

        private IEnumerator EmoteDelayedCR()
        {
            float delay = Random.Range(0, maxEmoteDelay);
            yield return new WaitForSeconds(delay);

            Emote();
        }
    }
}
