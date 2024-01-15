using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.AliveObjects
{
    public class AnimalManager : MonoBehaviour
    {
        public static AnimalManager Instance { get; private set; }
        public List<AnimalBehavior> AnimalList { get; set; }

        [SerializeField] private AnimalSo[] animalSoList;
        [SerializeField] private AliveObjectSo[] aliveObjectSoList;
        [SerializeField] public int boundsWidth = 20;
        [SerializeField] public int boundsHeight = 20;

        private float _spawnGrassTimer;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one AnimalManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            AnimalList = new List<AnimalBehavior>();
        }

        private void Start()
        {
            // Spawn initial animals
            foreach (AnimalSo animalSo in animalSoList)
            {
                for (int i = 0; i < animalSo.initialCount; i++)
                {
                    AnimalBehavior animalBehavior = Instantiate(animalSo.prefab, GetRandomPosition(), Quaternion.identity)
                        .GetComponent<AnimalBehavior>();
                    Vector3 position = animalBehavior.transform.position;
                    Quaternion rotation = animalBehavior.transform.rotation;
                    Vector3 targetDirection = GetRandomDirection();
                    float changeDirectionCooldown = Random.Range(1f, 5f);
                    AnimalData animalData = new(animalSo, position, rotation, targetDirection, changeDirectionCooldown);
                    animalBehavior.AnimalData = animalData;
                    AnimalList.Add(animalBehavior);
                }
            }

            // Spawn initial alive objects
            foreach (AliveObjectSo aliveObjectSo in aliveObjectSoList)
            {
                for (int i = 0; i < aliveObjectSo.initialCount; i++)
                    Instantiate(aliveObjectSo.prefab, GetRandomPosition(), Quaternion.identity);
            }
        }

        private void Update()
        {
            SpawnRandomGrass();
        }

        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(-boundsWidth, boundsWidth);
            float z = Random.Range(-boundsHeight, boundsHeight);
            return new Vector3(x, 0f, z);
        }

        private Vector3 GetRandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            return new Vector3(x, 0f, z).normalized;
        }

        private AnimalBehavior CreateAnimal(AliveObjectSo.Type type, Vector3 position, Quaternion rotation)
        {
            return Instantiate(animalSoList[(int)type].prefab, position, rotation).GetComponent<AnimalBehavior>();
        }

        public void AddAnimal(AnimalData newAnimalData)
        {
            AnimalBehavior newAnimal = CreateAnimal(newAnimalData.Type, newAnimalData.Position, newAnimalData.Rotation);
            newAnimal.AnimalData = newAnimalData;
            AnimalList.Add(newAnimal);
        }

        private void SpawnRandomGrass()
        {
            foreach (AliveObjectSo aliveObjectSo in aliveObjectSoList)
            {
                if (_spawnGrassTimer >= aliveObjectSo.tryReproduceRate)
                {
                    Instantiate(aliveObjectSo.prefab, GetRandomPosition(), Quaternion.identity);
                    _spawnGrassTimer = 0f;
                }
                else
                    _spawnGrassTimer += Time.deltaTime;
            }
        }
    }
}