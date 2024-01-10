using System;
using UnityEngine;

namespace _Scripts.Animals
{
    public class AnimalBehavior : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void Dead()
        {
            animator.Play("Dead");
        }
    }
}