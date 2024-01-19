using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.ScriptableObjects;
using _Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
using static _Scripts.Utils.HelperMethods;

namespace _Scripts.AliveObjects
{
    public class AnimalManager : MonoBehaviour
    {
        public static AnimalManager Instance { get; private set; }
        public List<AnimalBehavior> AnimalBehaviorList { get; private set; }
        public Dictionary<AliveObjectSo.Type, int> AliveObjectCount { get; private set; }
        public Action OnAliveObjectCountChanged { get; set; }

        [SerializeField] public AnimalSo[] animalSoList;
        [SerializeField] private AliveObjectSo[] aliveObjectSoList;
        [SerializeField] public int boundsWidth = 20;
        [SerializeField] public int boundsHeight = 20;
        [SerializeField] public float hungerDecayRate = 1f;
        [SerializeField] private float exportInterval = 1f;
        [SerializeField] private string fileName = "animal_data";

        private float _timeSinceStart;
        private float _spawnGrassTimer;
        private float _exportTimer;
        private string _filePath;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one AnimalManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            AnimalBehaviorList = new List<AnimalBehavior>();
            AliveObjectCount = new Dictionary<AliveObjectSo.Type, int>();
            _filePath = $"{Application.dataPath}/JsonFiles/{fileName}.json";
        }

        private void Start()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
                File.Delete($"{Application.dataPath}/JsonFiles/{fileName}.meta");
            }
            
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
                    AddNewAnimal(ref animalData);
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
            CheckForKeyPresses();
            
            _timeSinceStart += Time.deltaTime;
            SpawnRandomGrass();

            _exportTimer += Time.deltaTime;
            if (_exportTimer >= exportInterval)
            {
                _exportTimer = 0f;
                ExportData();
            }
        }

        private void CheckForKeyPresses()
        {
            if (Input.GetKey(KeyCode.Space))
                SetTimeScale(0f);
            if (Input.GetKey(KeyCode.Alpha1))
                SetTimeScale(1f);
            if (Input.GetKey(KeyCode.Alpha2))
                SetTimeScale(2f);
            if (Input.GetKey(KeyCode.Alpha3))
                SetTimeScale(4f);
            if (Input.GetKey(KeyCode.Alpha4))
                SetTimeScale(8f);
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

        public void AddNewAnimal(ref AnimalData newAnimalData)
        {
            AnimalBehavior newAnimal = Instantiate(animalSoList[(int)newAnimalData.Type].prefab, newAnimalData.Position,
                newAnimalData.Rotation).GetComponent<AnimalBehavior>();
            newAnimal.transform.localScale = new Vector3(newAnimalData.Size, newAnimalData.Size, newAnimalData.Size);
            newAnimal.animalData = newAnimalData;
            AnimalBehaviorList.Add(newAnimal);
            UpdateAliveObjectCount(newAnimal.animalData.Type, 1);
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

            OnAliveObjectCountChanged?.Invoke();
        }

        private void ExportData()
        {
            // Load existing data if the file exists
            WrapperHolder wrapperHolder;
            
            if (File.Exists(_filePath))
                wrapperHolder = ReadFromJson<WrapperHolder>(_filePath) ?? new WrapperHolder();
            else
                wrapperHolder = new WrapperHolder();
            
            List<AnimalData> animalDataList = AnimalBehaviorList.Select(x => x.animalData).ToList();
            // Convert the list to JSON and save it
            AnimalDataWrapper animalWrapperSave = new(animalDataList, _timeSinceStart);
            wrapperHolder.list.Add(animalWrapperSave);
            ExportToJson(wrapperHolder, _filePath);
        }

        public void SetTimeScale(float timeScale) => Time.timeScale = timeScale;
    }
}