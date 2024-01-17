using System.Linq;
using _Scripts.AliveObjects.Behaviors;
using _Scripts.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;
using static _Scripts.Utils.HelperMethods;

namespace _Scripts.AliveObjects
{
    [SelectionBase]
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

        private void Start()
        {
            animator.speed = Map(AnimalData.Genes[0], 0f, 1f, 2f, 0.5f);
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
            if (other.TryGetComponent(out Grass grass) && AnimalData.AnimalSo.canEatList.Contains(AliveObjectSo.Type.Grass))
            {
                AnimalData.Hunger += Mathf.InverseLerp(0, 100, grass.aliveObjectSo.hungerIncreaseWhenEaten) * 100;
                Destroy(grass.gameObject);
            }

            // Check if collided with another animal and if can eat it 
            if (other.TryGetComponent(out AnimalBehavior otherAnimal) && otherAnimal.AnimalData.IsActive &&
                AnimalData.AnimalSo.canEatList.Contains(otherAnimal.AnimalData.Type))
            {
                otherAnimal.Dead();
                AnimalData.Hunger += otherAnimal.AnimalData.HungerIncreaseWhenEaten;
                AnimalData.Hunger = Mathf.Clamp(AnimalData.Hunger, 0f, 100f);
            }
        }

        public void Dead()
        {
            // Play dead animation
            animator.Play("Dead");
            AnimalManager.Instance.UpdateAliveObjectCount(AnimalData.Type, -1);
            AnimalManager.Instance.AnimalList.Remove(this);
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
            if (_isDead) return;
            // Check if animal can reproduce
            AnimalData.ReproduceCooldown += Time.deltaTime;
            if (AnimalData.ReproduceCooldown < AnimalData.TryReproduceRate) return;
            AnimalData.ReproduceCooldown = 0;
            float chance = Random.Range(0, 100f) + AnimalData.TimeAlive;
            // Reproduce with 1% chance
            if (chance < 1f)
            {
                AnimalData newAnimalData = AnimalData.Copy();
                AnimalManager.Instance.AddNewAnimal(newAnimalData);
            }
        }
    }
}