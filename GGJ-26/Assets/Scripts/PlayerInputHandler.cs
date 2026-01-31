using UnityEngine;
using UnityEngine.InputSystem;

namespace Mask.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerAttackController))]
    [RequireComponent(typeof(PlayerEmoteController))]

    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovementController movementController;
        private PlayerAttackController attackController;
        private PlayerEmoteController emoteController;
        public void OnMove(InputValue value) => movementController.SetMoveInput(value.Get<Vector2>());
        public void OnEmote(InputValue value) => emoteController.Emote();
        public void OnAttack(InputValue value) => attackController.Attack();

        void Awake()
        {
            movementController = GetComponent<PlayerMovementController>();
            attackController = GetComponent<PlayerAttackController>();
            emoteController = GetComponent<PlayerEmoteController>();
        }
    }
}
