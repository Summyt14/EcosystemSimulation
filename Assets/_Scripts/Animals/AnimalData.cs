using System;
using _Scripts.ScriptableObjects;
using Unity.Collections;
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

        public AnimalType Type;
        public float Size;
        public float Hunger;
        public float TimeAlive;
        public bool IsActive;
        public float3 Position;
        public quaternion Rotation;
        public float3 TargetDirection;
        public float ChangeDirectionCooldown;
        public float ReproduceCooldown;
        [ReadOnly] public float Speed;
        [ReadOnly] public float RotationSpeed;
        [ReadOnly] public float EatDistance;
        [ReadOnly] public float HungerDecayRate;
        [ReadOnly] public float TryReproduceRate;
        [ReadOnly] public int HungerIncreaseWhenEaten;

        public AnimalData(AnimalType animalType, float3 position, quaternion rotation, float3 targetDirection,
            float changeDirectionCooldown, float speed, float rotationSpeed, float eatDistance, float hungerDecayRate, 
            float tryReproduceRate, int hungerIncreaseWhenEaten)
        {
            Type = animalType;
            Size = 1f;
            Hunger = 100f;
            TimeAlive = 0;
            IsActive = true;
            Position = position;
            Rotation = rotation;
            TargetDirection = targetDirection;
            ChangeDirectionCooldown = changeDirectionCooldown;
            ReproduceCooldown = 0;
            Speed = speed;
            RotationSpeed = rotationSpeed;
            EatDistance = eatDistance;
            HungerDecayRate = hungerDecayRate;
            TryReproduceRate = tryReproduceRate;
            HungerIncreaseWhenEaten = hungerIncreaseWhenEaten;
        }

        public AnimalData Copy()
        {
            return new AnimalData(Type, Position, Rotation, TargetDirection, ChangeDirectionCooldown,
                Speed, RotationSpeed, EatDistance, HungerDecayRate, TryReproduceRate, HungerIncreaseWhenEaten);
        }
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