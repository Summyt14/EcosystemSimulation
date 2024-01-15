using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.AliveObjects
{
    public struct AnimalData
    {
        public readonly AnimalSo AnimalSo;
        public AliveObjectSo.Type Type;
        public float Size;
        public float Hunger;
        public float TimeAlive;
        public bool IsActive;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 TargetDirection;
        public float ChangeDirectionCooldown;
        public float ReproduceCooldown;
        public readonly float Speed;
        public readonly float RotationSpeed;
        public readonly float HungerDecayRate;
        public readonly float TryReproduceRate;
        public readonly int HungerIncreaseWhenEaten;

        public AnimalData(AnimalSo animalSo, Vector3 position, Quaternion rotation, Vector3 targetDirection,
            float changeDirectionCooldown)
        {
            AnimalSo = animalSo;
            Type = animalSo.type;
            Size = 1f;
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
        }

        public AnimalData Copy() => new(AnimalSo, Position, Rotation, TargetDirection, ChangeDirectionCooldown);
    }
}