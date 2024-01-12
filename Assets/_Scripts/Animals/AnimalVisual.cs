using System;
using UnityEngine;

namespace _Scripts.Animals
{
    [SelectionBase]
    public class AnimalVisual : MonoBehaviour
    {
        public AnimalData AnimalData { get; set; }
        
        [SerializeField] private Animator animator;
        [SerializeField] private float timeUntilDestroy = 4f;

        private float _deadCountdown;
        private bool _isDead;

        private void Awake()
        {
            _deadCountdown = timeUntilDestroy;
        }

        private void Update()
        {
            // Check if animal is dead
            if (!_isDead && AnimalData.IsActive) return;
            _deadCountdown -= Time.deltaTime;
            // Destroy animal after time runs out
            if (_deadCountdown <= 0)
                Destroy(gameObject);
        }

        public void Dead()
        {
            // Play dead animation
            animator.Play("Dead");
            _isDead = true;
        }
    }
}