using UnityEngine;

namespace Mask.Player
{
    
    public class PlayerMovementController : PlayerControllerBase
    {
        [SerializeField] private float moveSpeed;
        private CharacterController controller;
        private Vector2 moveInput;

        new void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterController>();
        }

        public void SetMoveInput(Vector2 i)
        {
            moveInput = i;
        }

        void Move()
        {
            if (state.State != States.NONE) return;

            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
            controller.Move(movement * moveSpeed * Time.deltaTime);
        }

        void Update()
        {
            Move();
        }

    }
}
