using UnityEngine;

namespace Mask.Player
{
    public class PlayerControllerBase : MonoBehaviour
    {
        protected PlayerStates state;

        protected void Awake()
        {
            state = GetComponent<PlayerStates>();
        }
    }
}
