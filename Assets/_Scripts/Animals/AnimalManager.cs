using System.Collections.Generic;
using System.Linq;
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

        private List<AnimalData> _animalDataList;
        private List<AnimalVisual> _animalVisualList;
        private JobHandle _animalJobHandle;

        private void Start()
        {
            SpawnAnimals();
        }

        private void SpawnAnimals()
        {
            _animalDataList = new List<AnimalData>();
            _animalVisualList = new List<AnimalVisual>();

            // Spawn initial animals
            foreach (AnimalSo animalSo in animalSoList)
            {
                for (int i = 0; i < animalSo.initialCount; i++)
                {
                    AnimalVisual animalVisual = Instantiate(animalSo.prefab, GetRandomPosition(), Quaternion.identity)
                        .GetComponent<AnimalVisual>();
                    float3 position = animalVisual.transform.position;
                    quaternion rotation = animalVisual.transform.rotation;
                    float3 targetDirection = GetRandomDirection();
                    float changeDirectionCooldown = Random.Range(1f, 5f);
                    AnimalData animalData = new(animalSo.type, position, rotation, targetDirection,
                        changeDirectionCooldown, animalSo.initialSpeed, animalSo.initialRotationSpeed,
                        animalSo.initialEatDistance, animalSo.hungerDecayRate, animalSo.tryReproduceRate,
                        animalSo.hungerIncreaseWhenEaten);
                    animalVisual.AnimalData = animalData;
                    _animalDataList.Add(animalData);
                    _animalVisualList.Add(animalVisual);
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(-boundsWidth, boundsWidth);
            float z = Random.Range(-boundsHeight, boundsHeight);
            return new Vector3(x, 0f, z);
        }

        private float3 GetRandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            return math.normalize(new float3(x, 0f, z));
        }

        private void Update()
        {
            UpdateAnimals();
        }

        private void UpdateAnimals()
        {
            int seed = Random.Range(0, int.MaxValue);
            // Initialize lists
            AnimalData[] animalDataArray = _animalDataList.ToArray();
            NativeArray<AnimalData> animalDataNative = new(animalDataArray, Allocator.TempJob);
            NativeArray<AnimalData> newAnimalDataNative = new(animalDataArray.Length, Allocator.TempJob);

            AnimalMovementJob animalMovementJob = new()
            {
                AnimalDataNativeArray = animalDataNative,
                DeltaTime = Time.deltaTime,
                BoundsWidth = boundsWidth,
                BoundsHeight = boundsHeight,
                Seed = seed
            };

            AnimalStatsJob animalStatsJob = new()
            {
                AnimalDataNativeArray = animalDataNative,
                NewAnimalNativeArray = newAnimalDataNative,
                DeltaTime = Time.deltaTime,
                Seed = seed
            };

            // Run jobs
            _animalJobHandle = animalMovementJob.Schedule(animalDataArray.Length, 32, _animalJobHandle);
            _animalJobHandle = animalStatsJob.Schedule(animalDataArray.Length, 32, _animalJobHandle);
            _animalJobHandle.Complete();
            
            // Update list
            animalDataNative.CopyTo(animalDataArray);
            animalDataNative.Dispose();
            _animalDataList.Clear();
            _animalDataList.AddRange(animalDataArray);

            // Create new animals
            for (int i = 0; i < newAnimalDataNative.Length; i++)
            {
                if (!newAnimalDataNative[i].IsActive) continue;
                _animalDataList.Add(newAnimalDataNative[i]);
                AnimalVisual newAnimalVisual = CreateAnimal(newAnimalDataNative[i].Type, newAnimalDataNative[i].Position, newAnimalDataNative[i].Rotation)
                    .GetComponent<AnimalVisual>();
                newAnimalVisual.AnimalData = newAnimalDataNative[i];
                _animalVisualList.Add(newAnimalVisual);
                Debug.Log("New animal");
            }

            newAnimalDataNative.Dispose();
            
            List<int> indicesToRemove = new();
            for (int i = 0; i < _animalDataList.Count; i++)
            {
                // Check if animal is dead
                if (!_animalDataList[i].IsActive)
                {
                    // If so, remove it from the list
                    _animalVisualList[i].Dead();
                    indicesToRemove.Add(i);
                }
                else
                {
                    // If not, update its position
                    _animalVisualList[i].transform.position = _animalDataList[i].Position;
                    _animalVisualList[i].transform.rotation = _animalDataList[i].Rotation;
                }
            }

            // Remove dead animals
            foreach (int index in indicesToRemove)
            {
                RemoveAnimal(index);
            }
        }

        private GameObject CreateAnimal(AnimalData.AnimalType animalType, Vector3 position, Quaternion rotation)
        {
            return Instantiate(animalSoList[(int)animalType].prefab, position, rotation);
        }

        private void RemoveAnimal(int index)
        {
            _animalDataList.RemoveAt(index);
            _animalVisualList.RemoveAt(index);
        }
    }
}