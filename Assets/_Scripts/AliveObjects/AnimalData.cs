using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using static _Scripts.Utils.HelperMethods;
using Random = UnityEngine.Random;

namespace _Scripts.AliveObjects
{
    [Serializable]
    public struct AnimalData
    {
        [NonSerialized] public readonly AnimalSo AnimalSo;
        public AliveObjectSo.Type Type;
        public float[] Genes;
        public float TimeAlive;
        public float Speed;
        public float Size;
        public float ReproduceRate;
        public float Fitness;
        [NonSerialized] public float Hunger;
        [NonSerialized] public bool IsActive;
        [NonSerialized] public Vector3 Position;
        [NonSerialized] public Quaternion Rotation;
        [NonSerialized] public Vector3 TargetDirection;
        [NonSerialized] public float ChangeDirectionCooldown;
        [NonSerialized] public float ReproduceCooldown;
        [NonSerialized] public float RotationSpeed;
        [NonSerialized] public float HungerIncreaseRate;
        [NonSerialized] public int HungerDecreaseWhenEaten;

        public AnimalData(AnimalSo animalSo, Vector3 position, Quaternion rotation, Vector3 targetDirection,
            float changeDirectionCooldown)
        {
            AnimalSo = animalSo;
            Type = animalSo.type;
            Genes = new float[2];
            TimeAlive = 0;
            Speed = 0f;
            Size = 0f;
            ReproduceRate = 0f;
            Fitness = 0f;
            Hunger = 0f;
            IsActive = true;
            Position = position;
            Rotation = rotation;
            TargetDirection = targetDirection;
            ChangeDirectionCooldown = changeDirectionCooldown;
            ReproduceCooldown = 0;
            RotationSpeed = animalSo.initialRotationSpeed;
            ReproduceRate = animalSo.tryReproduceRate;
            HungerIncreaseRate = 0f;
            HungerDecreaseWhenEaten = animalSo.hungerDecreaseWhenEaten;

            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = Random.Range(0f, 1f);
            
            MapGenes();
        }

        public void MapGenes()
        {
            Speed = Map(Genes[0], 0f, 1f, 10f, 1f);
            Size = Map(Genes[0], 0f, 1f, 0.5f, 2f);
            //ReproduceRate = Map(Genes[1], 0f, 1f, 0f, 100f);
            HungerIncreaseRate = Speed;
        }

        public AnimalData Reproduce()
        {
            CalculateFitness();
            float[] newGenes = Mutate(Genes, ReproduceRate);
            AnimalData newAnimalData = new(AnimalSo, Position, Rotation, TargetDirection, ChangeDirectionCooldown)
            {
                Genes = newGenes
            };
            newAnimalData.MapGenes();
            return newAnimalData;
        }

        private void CalculateFitness()
        {
            float fitness = TimeAlive - Mathf.Clamp01(HungerIncreaseRate) + Mathf.Pow(2, Mathf.Clamp01(ReproduceRate));
            Fitness = Mathf.Max(0, fitness);
        }

        private float[] Mutate(IReadOnlyList<float> parentGenes, float mutationRate)
        {
            float[] mutatedGenes = new float[parentGenes.Count];
            for (int i = 0; i < parentGenes.Count; i++)
            {
                if (Random.Range(0, 100f) < mutationRate)
                    mutatedGenes[i] = Random.Range(0f, 1f);
                else
                    mutatedGenes[i] = parentGenes[i];
            }

            return mutatedGenes;
        }
    }
}