using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Mask.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementController : PlayerControllerBase
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float acceleration = 15f;
        public float rotationSpeed = 10f;

        [Header("Crowd Physics")]
        public float pushForce = 7.0f;
        public float collisionSlowdown = 0.5f;
        public float recoveryRate = 2.0f;

        private Rigidbody rb;
        private NavMeshAgent navAgent;
        private Vector2 moveInput;
        private bool isEmoteActive;
        private float currentSpeedModifier = 1.0f;
        private bool isAttackActive;

        new void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();

            // Setup Rigidbody for physical interactions
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearDamping = 5f; // Friction so you don't slide forever
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }

        void FixedUpdate()
        {
            
            Move();
        }

        public void SetMoveInput(Vector2 i)
        {
            moveInput = i;
        }

        void Move()
        {
            Debug.Log(state.State);
            if (state.State != States.NONE) return;

            // Calculate direction based on input
            Vector3 targetDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 targetVelocity = targetDirection * moveSpeed;

            // Apply force to reach target velocity (Physical movement)
            Vector3 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange.y = 0; // Don't interfere with gravity

            rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
            {
                currentSpeedModifier = collisionSlowdown;

                Vector3 pushDir = transform.position - collision.transform.position;
                pushDir.y = 0;
                pushDir.Normalize();

                // Push self away
                rb.AddForce(pushDir * pushForce, ForceMode.Acceleration);

                // Push the agent away (Mutual force)
                if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody agentRb))
                {
                    agentRb.AddForce(-pushDir * (pushForce * 0.5f), ForceMode.Acceleration);
                }
            }
        }
    }
}
