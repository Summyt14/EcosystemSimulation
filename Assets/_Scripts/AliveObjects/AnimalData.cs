using _Scripts.ScriptableObjects;
using UnityEngine;
using static _Scripts.Utils.HelperMethods;

namespace _Scripts.AliveObjects
{
    public struct AnimalData
    {
        public readonly AnimalSo AnimalSo;
        public readonly AliveObjectSo.Type Type;
        public float[] Genes;
        public float Hunger;
        public float TimeAlive;
        public bool IsActive;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 TargetDirection;
        public float ChangeDirectionCooldown;
        public float ReproduceCooldown;
        public readonly float Speed;
        public readonly float Size;
        public readonly float RotationSpeed;
        public readonly float HungerDecayRate;
        public readonly float TryReproduceRate;
        public readonly int HungerIncreaseWhenEaten;

        public AnimalData(AnimalSo animalSo, Vector3 position, Quaternion rotation, Vector3 targetDirection,
            float changeDirectionCooldown, int genesLenght)
        {
            AnimalSo = animalSo;
            Type = animalSo.type;
            Genes = new float[genesLenght];
            Hunger = 100f;
            TimeAlive = 0;
            IsActive = true;
            Position = position;
            Rotation = rotation;
            TargetDirection = targetDirection;
            ChangeDirectionCooldown = changeDirectionCooldown;
            ReproduceCooldown = 0;
            Speed = animalSo.initialSpeed;
            RotationSpeed = animalSo.initialRotationSpeed;
            HungerDecayRate = animalSo.hungerDecayRate;
            TryReproduceRate = animalSo.tryReproduceRate;
            HungerIncreaseWhenEaten = animalSo.hungerIncreaseWhenEaten;

            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = Random.Range(0f, 1f);
            
            Speed = Map(Genes[0], 0f, 1f, 10f, 0f);
            Size = Map(Genes[0], 0f, 1f, 0.5f, 2f);
        }

        public AnimalData Copy() => new(AnimalSo, Position, Rotation, TargetDirection, ChangeDirectionCooldown, Genes.Length);
    }
}