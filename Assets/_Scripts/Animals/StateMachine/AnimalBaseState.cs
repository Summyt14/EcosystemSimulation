using UnityEngine;

namespace _Scripts.Animals.StateMachine
{
    public abstract class AnimalBaseState : IState
    {
        protected readonly Animal Animal;
        protected readonly Animator Animator;
        
        // Animation Hashes
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int DieHash = Animator.StringToHash("Die");
        
        protected const float crossFadeDuration = 0.1f;
        
        protected AnimalBaseState(Animal animal, Animator animator)
        {
            Animal = animal;
            Animator = animator;
        }
        
        public virtual void OnEnter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}