using System;
using _Scripts.ScriptableObjects;
using UnityEngine;
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
        [NonSerialized] public float Hunger;
        [NonSerialized] public bool IsActive;
        [NonSerialized] public Vector3 Position;
        [NonSerialized] public Quaternion Rotation;
        [NonSerialized] public Vector3 TargetDirection;
        [NonSerialized] public float ChangeDirectionCooldown;
        [NonSerialized] public float ReproduceCooldown;
        [NonSerialized] public float RotationSpeed;
        [NonSerialized] public float HungerDecayRate;
        [NonSerialized] public float TryReproduceRate;
        [NonSerialized] public int HungerIncreaseWhenEaten;

        public AnimalData(AnimalSo animalSo, Vector3 position, Quaternion rotation, Vector3 targetDirection,
            float changeDirectionCooldown, int genesLenght)
        {
            AnimalSo = animalSo;
            Type = animalSo.type;
            Genes = new float[genesLenght];
            TimeAlive = 0;
            Hunger = 100f;
            IsActive = true;
            Position = position;
            Rotation = rotation;
            TargetDirection = targetDirection;
            ChangeDirectionCooldown = changeDirectionCooldown;
            ReproduceCooldown = 0;
            RotationSpeed = animalSo.initialRotationSpeed;
            TryReproduceRate = animalSo.tryReproduceRate;
            HungerIncreaseWhenEaten = animalSo.hungerIncreaseWhenEaten;

            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = Random.Range(0f, 1f);
            
            Speed = Map(Genes[0], 0f, 1f, 10f, 1f);
            Size = Map(Genes[0], 0f, 1f, 0.5f, 2f);
            HungerDecayRate = Speed;
        }

        public AnimalData Copy() => new(AnimalSo, Position, Rotation, TargetDirection, ChangeDirectionCooldown, Genes.Length);
    }
}