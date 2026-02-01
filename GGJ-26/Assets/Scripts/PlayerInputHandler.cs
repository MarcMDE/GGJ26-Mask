using UnityEngine;
using UnityEngine.InputSystem;

namespace Mask.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerAttackController))]
    [RequireComponent(typeof(EmoteController))]
    [RequireComponent(typeof(MaskSwitchController))]

    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovementController movementController;
        private PlayerAttackController attackController;
        private EmoteController emoteController;
        private MaskSwitchController maskSwitchController;
        public void OnMove(InputValue value) => movementController.SetMoveInput(value.Get<Vector2>());
        public void OnEmote(InputValue value) => emoteController.Emote();
        public void OnAttack(InputValue value) => attackController.Attack();
        public void OnSwitch(InputValue value) => maskSwitchController.TrySwitchMask();

        void Awake()
        {
            movementController = GetComponent<PlayerMovementController>();
            attackController = GetComponent<PlayerAttackController>();
            emoteController = GetComponent<EmoteController>();
            maskSwitchController = GetComponent<MaskSwitchController>();
        }
    }
}
