using _Scripts.AliveObjects.Behaviors;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.AliveObjects
{
    public class AnimalBehavior : MonoBehaviour
    {
        public AnimalData AnimalData;

        [SerializeField] private Animator animator;
        [SerializeField] private float timeUntilDestroy = 4f;

        private float _deadCountdown;
        private bool _isDead;
        private RandomMoveBehavior _randomMoveBehavior;

        private void Awake()
        {
            _deadCountdown = timeUntilDestroy;
            _randomMoveBehavior =
                new RandomMoveBehavior(AnimalManager.Instance.boundsWidth, AnimalManager.Instance.boundsHeight);
        }

        private void Update()
        {
            // Check if animal is dead
            if (_isDead && !AnimalData.IsActive)
            {
                _deadCountdown -= Time.deltaTime;
                // Destroy animal after time runs out
                if (_deadCountdown <= 0)
                    Destroy(gameObject);

                return;
            }

            _randomMoveBehavior.Update(ref AnimalData, Time.deltaTime);
            HandleStats();
            TryReproduce();

            transform.position = AnimalData.Position;
            transform.rotation = AnimalData.Rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if collided with grass and if can eat it
            if (other.TryGetComponent(out Grass grass) && AnimalData.AnimalSo.type.CanEat(AliveObjectSo.Type.Grass))
            {
                AnimalData.Hunger += Mathf.InverseLerp(0, 100, grass.aliveObjectSo.hungerIncreaseWhenEaten) * 100;
                Destroy(grass.gameObject);
            }

            // Check if collided with another animal and if can eat it 
            if (other.TryGetComponent(out AnimalBehavior otherAnimal) && otherAnimal.AnimalData.IsActive &&
                AnimalData.AnimalSo.type.CanEat(otherAnimal.AnimalData.Type))
            {
                otherAnimal.Dead();
                AnimalData.Hunger += Mathf.InverseLerp(0, 100, otherAnimal.AnimalData.HungerIncreaseWhenEaten) * 100;
            }
        }

        public void Dead()
        {
            // Play dead animation
            animator.Play("Dead");
            _isDead = true;
            AnimalData.IsActive = false;
        }

        private void HandleStats()
        {
            // Check if animal is fully hungry and if so deactivate it
            if (AnimalData.Hunger <= 0)
            {
                Dead();
                return;
            }

            // Update stats based on time since last update and decrease hunger
            AnimalData.TimeAlive += Time.deltaTime;
            AnimalData.Hunger -= AnimalData.HungerDecayRate * Time.deltaTime;
        }

        private void TryReproduce()
        {
            // Check if animal can reproduce
            AnimalData.ReproduceCooldown += Time.deltaTime;
            if (AnimalData.ReproduceCooldown < AnimalData.TryReproduceRate) return;
            AnimalData.ReproduceCooldown = 0;
            float chance = Random.Range(0, 100f);
            // Reproduce with 10% chance
            if (chance >= 90f)
            {
                AnimalData newAnimalData = AnimalData.Copy();
                //TODO mutate
                AnimalManager.Instance.AddAnimal(newAnimalData);
            }
        }
    }
}