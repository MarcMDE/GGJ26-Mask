using UnityEngine;

namespace Mask.Player
{
    public enum States{ NONE = 0, ATTACK, EMOTE, DEAD };
    public class PlayerStates : MonoBehaviour
    {
        public States State { get; private set; }

        void Start()
        {
            State = States.NONE;
        }

        public bool TrySetState(States state)
        {
            if (state == States.ATTACK && State != States.NONE) return false;
            if (state == States.EMOTE && State != States.NONE) return false;

            State = state;
            return true;
        }  
    }
}
