using UnityEngine;
using System.Collections;

namespace Mask.Player
{
    public class EmoteController : MonoBehaviour
    {
        private Animator animator;

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
    }
}
