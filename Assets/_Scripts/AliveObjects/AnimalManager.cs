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
        public Dictionary<AliveObjectSo.Type, int> AliveObjectCount { get; set; }
        public Action OnAliveObjectCountChanged { get; set; }
        
        [SerializeField] public AnimalSo[] animalSoList;
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
            AliveObjectCount = new Dictionary<AliveObjectSo.Type, int>();
        }

        private void Start()
        {
            // Spawn initial animals
            foreach (AnimalSo animalSo in animalSoList)
            {
                for (int i = 0; i < animalSo.initialCount; i++)
                {
                    Vector3 position = GetRandomPosition();
                    Quaternion rotation = Quaternion.identity;
                    Vector3 targetDirection = GetRandomDirection();
                    float changeDirectionCooldown = Random.Range(1f, 5f);
                    AnimalData animalData = new(animalSo, position, rotation, targetDirection, changeDirectionCooldown, 1);
                    AddNewAnimal(animalData);
                }
            }

            // Spawn initial alive objects
            foreach (AliveObjectSo aliveObjectSo in aliveObjectSoList)
            {
                for (int i = 0; i < aliveObjectSo.initialCount; i++)
                {
                    Instantiate(aliveObjectSo.prefab, GetRandomPosition(), Quaternion.identity);
                    UpdateAliveObjectCount(aliveObjectSo.type, 1);
                }
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

        public void AddNewAnimal(AnimalData newAnimalData)
        {
            AnimalBehavior newAnimal = Instantiate(animalSoList[(int)newAnimalData.Type].prefab, newAnimalData.Position,
                newAnimalData.Rotation).GetComponent<AnimalBehavior>();
            newAnimal.transform.localScale = new Vector3(newAnimalData.Size, newAnimalData.Size, newAnimalData.Size);
            newAnimal.AnimalData = newAnimalData;
            AnimalList.Add(newAnimal);
            UpdateAliveObjectCount(newAnimalData.Type, 1);
        }

        private void SpawnRandomGrass()
        {
            foreach (AliveObjectSo aliveObjectSo in aliveObjectSoList)
            {
                if (_spawnGrassTimer >= aliveObjectSo.tryReproduceRate)
                {
                    Instantiate(aliveObjectSo.prefab, GetRandomPosition(), Quaternion.identity);
                    _spawnGrassTimer = 0f;
                    UpdateAliveObjectCount(AliveObjectSo.Type.Grass, 1);
                }
                else
                    _spawnGrassTimer += Time.deltaTime;
            }
        }

        public void UpdateAliveObjectCount(AliveObjectSo.Type type, int amount)
        {   
            if (!AliveObjectCount.ContainsKey(type))
                AliveObjectCount.Add(type, amount);
            else
                AliveObjectCount[type] += amount;
            
            OnAliveObjectCountChanged.Invoke();
        }
    }
}