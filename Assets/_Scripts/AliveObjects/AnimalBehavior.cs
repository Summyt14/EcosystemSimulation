using System.Linq;
using _Scripts.AliveObjects.Behaviors;
using _Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using static _Scripts.Utils.HelperMethods;

namespace _Scripts.AliveObjects
{
    [SelectionBase]
    public class AnimalBehavior : MonoBehaviour
    {
        public AnimalData animalData;

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

        private void Start()
        {
            animator.speed = Map(animalData.Genes[0], 0f, 1f, 2f, 0.5f);
        }

        private void Update()
        {
            // Check if animal is dead
            if (_isDead && !animalData.IsActive)
            {
                _deadCountdown -= Time.deltaTime;
                // Destroy animal after time runs out
                if (_deadCountdown <= 0)
                    Destroy(gameObject);

                return;
            }

            _randomMoveBehavior.Update(ref animalData, Time.deltaTime);
            HandleStats();
            TryReproduce();

            transform.position = animalData.Position;
            transform.rotation = animalData.Rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if collided with grass and if can eat it
            if (other.TryGetComponent(out Grass grass) && animalData.AnimalSo.canEatList.Contains(AliveObjectSo.Type.Grass))
            {
                animalData.Hunger += grass.aliveObjectSo.hungerIncreaseWhenEaten * AnimalManager.Instance.hungerDecayRate;
                animalData.Hunger = Mathf.Clamp(grass.aliveObjectSo.hungerIncreaseWhenEaten, 0f, 100f);
                Destroy(grass.gameObject);
                AnimalManager.Instance.UpdateAliveObjectCount(AliveObjectSo.Type.Grass, -1);
            }

            // Check if collided with another animal and if can eat it 
            if (other.TryGetComponent(out AnimalBehavior otherAnimal) && otherAnimal.animalData.IsActive &&
                animalData.AnimalSo.canEatList.Contains(otherAnimal.animalData.Type))
            {
                otherAnimal.Dead();
                animalData.Hunger += otherAnimal.animalData.HungerIncreaseWhenEaten * AnimalManager.Instance.hungerDecayRate;
                animalData.Hunger = Mathf.Clamp(animalData.Hunger, 0f, 100f);
            }
        }

        public void Dead()
        {
            // Play dead animation
            animator.Play("Dead");
            AnimalManager.Instance.UpdateAliveObjectCount(animalData.Type, -1);
            AnimalManager.Instance.AnimalBehaviorList.Remove(this);
            _isDead = true;
            animalData.IsActive = false;
        }

        private void HandleStats()
        {
            // Check if animal is fully hungry and if so deactivate it
            if (animalData.Hunger <= 0)
            {
                Dead();
                return;
            }

            // Update stats based on time since last update and decrease hunger
            animalData.TimeAlive += Time.deltaTime;
            animalData.Hunger -= animalData.HungerDecayRate * Time.deltaTime;
        }

        private void TryReproduce()
        {
            if (_isDead) return;
            // Check if animal can reproduce
            animalData.ReproduceCooldown += Time.deltaTime;
            if (animalData.ReproduceCooldown < animalData.TryReproduceRate) return;
            animalData.ReproduceCooldown = 0;
            float chance = Mathf.Clamp(Random.Range(0, 100f) + animalData.TimeAlive, 0f, 100f);
            // Reproduce with 1% chance
            if (chance >= 99f)
            {
                AnimalData newAnimalData = animalData.Copy();
                AnimalManager.Instance.AddNewAnimal(ref newAnimalData);
            }
        }
    }
}