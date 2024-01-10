using System;
using Unity.Collections;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    public struct AnimalData
    {
        public enum AnimalType
        {
            Fox = 1,
            Chicken = 2
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
        [ReadOnly] public float Speed;
        [ReadOnly] public float RotationSpeed;
        [ReadOnly] public float EatDistance;
        [ReadOnly] public float HungerDecayRate;
        [ReadOnly] public int HungerIncreaseWhenEaten;

        public AnimalData(AnimalType animalType, float3 position, quaternion rotation, float3 targetDirection,
            float changeDirectionCooldown, float speed, float rotationSpeed, float eatDistance, float hungerDecayRate,
            int hungerIncreaseWhenEaten)
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
            Speed = speed;
            RotationSpeed = rotationSpeed;
            EatDistance = eatDistance;
            HungerDecayRate = hungerDecayRate;
            HungerIncreaseWhenEaten = hungerIncreaseWhenEaten;
        }

        public AnimalData Copy()
        {
            return new AnimalData(Type, Position, Rotation, TargetDirection, ChangeDirectionCooldown,
                Speed, RotationSpeed, EatDistance, HungerDecayRate, HungerIncreaseWhenEaten);
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