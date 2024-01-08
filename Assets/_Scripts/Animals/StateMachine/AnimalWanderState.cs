using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Animals.StateMachine
{
    public class AnimalWanderState : AnimalBaseState
    {
        private readonly NavMeshAgent _agent;
        private readonly Vector3 _startPoint;
        private readonly float _wanderRadius;

        public AnimalWanderState(Animal animal, Animator animator, NavMeshAgent agent, float wanderRadius) : base(animal,
            animator)
        {
            _agent = agent;
            _startPoint = animal.transform.position;
            _wanderRadius = wanderRadius;
        }

        public override void OnEnter()
        {
            Animator.CrossFade(WalkHash, crossFadeDuration);
        }

        public override void Update()
        {
            if (!HasReachedDestination()) return;
            Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
            randomDirection += _startPoint;
            NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _wanderRadius, 1);
            _agent.SetDestination(hit.position);
        }

        private bool HasReachedDestination()
        {
            return !_agent.pathPending 
                   && _agent.remainingDistance <= _agent.stoppingDistance 
                   && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
        }
    }
}