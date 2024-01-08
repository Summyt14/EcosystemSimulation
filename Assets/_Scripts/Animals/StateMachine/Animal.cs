using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Animals.StateMachine
{
    public class Animal : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;

        private void Start()
        {
            AnimalBaseState wanderState = new AnimalWanderState(this, animator, agent, 5f);
        }
    }
}