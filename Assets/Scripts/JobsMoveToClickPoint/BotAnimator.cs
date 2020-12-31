using System;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    [RequireComponent(typeof(Animator))]
    public class BotAnimator : MonoBehaviour
    {
        // Animator properties
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int MoveForward = Animator.StringToHash("MoveForward");
        private static readonly int MoveRight = Animator.StringToHash("MoveRight");

        // Animator states
        private readonly int _idleStateHash = Animator.StringToHash("idle");
        private readonly int _walkingStateHash = Animator.StringToHash("move");

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            
        }

        public void ApplyMovement(Vector3 velocity)
        {
            if (velocity.magnitude > 0.1f)
            {
                _animator.SetFloat(MoveForward, velocity.z);
                _animator.SetFloat(MoveRight, velocity.x);
                _animator.SetBool(Move, true);
            }
            else
                _animator.SetBool(Move, false);
        }
    }
}