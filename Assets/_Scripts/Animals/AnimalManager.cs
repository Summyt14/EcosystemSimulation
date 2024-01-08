using System;
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
        [SerializeField] private uint seed = 123;

        private int _animalCount;
        private NativeArray<Animal> _animals;
        private List<Transform> _animalTransforms;
        private JobHandle _animalJobHandle;

        private void Start()
        {
            SpawnAnimals();
        }

        private void SpawnAnimals()
        {
            foreach (AnimalSo animalSo in animalSoList)
                _animalCount += animalSo.initialCount;

            _animals = new NativeArray<Animal>(_animalCount, Allocator.Persistent);
            _animalTransforms = new List<Transform>();
            int index = 0;

            foreach (AnimalSo animalSo in animalSoList)
            {
                for (int i = 0; i < animalSo.initialCount; i++)
                {
                    GameObject animalGo = Instantiate(animalSo.prefab, GetRandomPosition(), Quaternion.identity);
                    _animalTransforms.Add(animalGo.transform);
                    float3 position = animalGo.transform.position;
                    quaternion rotation = animalGo.transform.rotation;
                    float3 targetDirection = GetRandomDirection();
                    float changeDirectionCooldown = UnityEngine.Random.Range(1f, 5f);
                    _animals[index++] = new Animal(animalSo.type, true, position, rotation, targetDirection, changeDirectionCooldown,
                        animalSo.initialSpeed, animalSo.initialRotationSpeed, animalSo.initialEatDistance, seed);
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
            MoveAnimals();
        }

        private void MoveAnimals()
        {
            AnimalMovementJob animalMovementJob = new()
            {
                Animals = _animals,
                DeltaTime = Time.deltaTime,
                BoundsWidth = boundsWidth,
                BoundsHeight = boundsHeight
            };

            AnimalEatJob animalEatJob = new()
            {
                Animals = _animals,
            };

            _animalJobHandle = animalMovementJob.Schedule(_animalCount, 32);
            _animalJobHandle = animalEatJob.Schedule(_animalCount, 32, _animalJobHandle);
        }

        private void LateUpdate()
        {
            _animalJobHandle.Complete();

            for (int i = 0; i < _animalCount; i++)
            {
                _animalTransforms[i].position = _animals[i].Position;
                _animalTransforms[i].rotation = _animals[i].Rotation;
            }
        }

        private void OnDestroy()
        {
            _animals.Dispose();
        }

        public void DeleteAnimal(int index)
        {
            Destroy(_animalTransforms[index].gameObject);
            //_animalTransforms.RemoveAt(index);
            _animalCount--;
        }
    }
}