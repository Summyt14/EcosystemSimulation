using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace _Scripts.Animals
{
    public class AnimalManager : MonoBehaviour
    {
        [SerializeField] private AnimalSo[] animalSoList;
        [SerializeField] private int boundsWidth = 20;
        [SerializeField] private int boundsHeight = 20;

        private int _animalCount;
        private NativeArray<AnimalData> _animals;
        private List<AnimalBehavior> _animalsBehavior;
        private JobHandle _animalJobHandle;

        private void Start()
        {
            SpawnAnimals();
        }

        private void SpawnAnimals()
        {
            foreach (AnimalSo animalSo in animalSoList)
                _animalCount += animalSo.initialCount;

            _animals = new NativeArray<AnimalData>(_animalCount, Allocator.Persistent);
            _animalsBehavior = new List<AnimalBehavior>();
            int index = 0;

            foreach (AnimalSo animalSo in animalSoList)
            {
                for (int i = 0; i < animalSo.initialCount; i++)
                {
                    AnimalBehavior animalGo = Instantiate(animalSo.prefab, GetRandomPosition(), Quaternion.identity)
                        .GetComponent<AnimalBehavior>();
                    _animalsBehavior.Add(animalGo);
                    float3 position = animalGo.transform.position;
                    quaternion rotation = animalGo.transform.rotation;
                    float3 targetDirection = GetRandomDirection();
                    float changeDirectionCooldown = UnityEngine.Random.Range(1f, 5f);
                    _animals[index++] = new AnimalData(animalSo.type, position, rotation, targetDirection,
                        changeDirectionCooldown, animalSo.initialSpeed, animalSo.initialRotationSpeed,
                        animalSo.initialEatDistance, animalSo.hungerDecayRate, animalSo.hungerIncreaseWhenEaten);
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            float x = UnityEngine.Random.Range(-boundsWidth, boundsWidth);
            float z = UnityEngine.Random.Range(-boundsHeight, boundsHeight);
            return new Vector3(x, 0f, z);
        }

        private float3 GetRandomDirection()
        {
            float x = UnityEngine.Random.Range(-1f, 1f);
            float z = UnityEngine.Random.Range(-1f, 1f);
            return math.normalize(new float3(x, 0f, z));
        }

        private void Update()
        {
            UpdateAnimals();
        }

        private void UpdateAnimals()
        {
            float randomFloat = Random.Range(0, 100f);
            AnimalMovementJob animalMovementJob = new()
            {
                Animals = _animals,
                DeltaTime = Time.deltaTime,
                BoundsWidth = boundsWidth,
                BoundsHeight = boundsHeight,
                RandomFloat = randomFloat
            };

            AnimalStatsJob animalStatsJob = new()
            {
                Animals = _animals,
                DeltaTime = Time.deltaTime,
                RandomFloat = randomFloat
            };

            _animalJobHandle = animalMovementJob.Schedule(_animalCount, 32, _animalJobHandle);
            _animalJobHandle = animalStatsJob.Schedule(_animalCount, 32, _animalJobHandle);
        }

        private void LateUpdate()
        {
            _animalJobHandle.Complete();

            for (int i = 0; i < _animalCount; i++)
            {
                if (!_animals[i].IsActive)
                    _animalsBehavior[i].Dead();
                
                _animalsBehavior[i].transform.position = _animals[i].Position;
                _animalsBehavior[i].transform.rotation = _animals[i].Rotation;
            }
        }

        private void OnDestroy()
        {
            _animals.Dispose();
        }

        public void DeleteAnimal(int index)
        {
            Destroy(_animalsBehavior[index].gameObject);
            //_animalTransforms.RemoveAt(index);
            _animalCount--;
        }
    }
}