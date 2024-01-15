using System;
using _Scripts.ScriptableObjects;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Animals
{
    public struct AnimalData
    {
        public enum AnimalType
        {
            Fox = 0,
            Chicken = 1
        }

        public AnimalSo AnimalSo;
        public AnimalType Type;
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

    public static class AnimalTypeExtensions
    {
        public static bool CanEat(this AnimalData.AnimalType animal1, AnimalData.AnimalType animal2)
        {
            switch (animal1)
            {
                case AnimalData.AnimalType.Fox:
                    if (animal2 == AnimalData.AnimalType.Chicken) return true;
                    break;
                case AnimalData.AnimalType.Chicken:
                    if (animal2 == AnimalData.AnimalType.Fox) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animal1), animal1, "Unknown animal type");
            }

            return false;
        }
    }
}