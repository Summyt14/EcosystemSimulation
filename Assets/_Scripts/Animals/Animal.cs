using System;
using _Scripts.ScriptableObjects;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct Animal
{
    public enum AnimalType
    {
        Fox = 1,
        Bunny = 2
    }

    public AnimalType Type;

    // public float size;
    // public float hunger;
    // public Collider collider;
    public bool IsActive;
    public float3 Position;
    public quaternion Rotation;
    public float3 TargetDirection;
    public float ChangeDirectionCooldown;
    [ReadOnly] public float Speed;
    [ReadOnly] public float RotationSpeed;
    [ReadOnly] public float EatDistance;
    [ReadOnly] public uint Seed;

    public Animal(AnimalType animalType, bool isActive, float3 position, quaternion rotation, float3 targetDirection,
        float changeDirectionCooldown,
        float speed, float rotationSpeed, float eatDistance, uint seed)
    {
        Type = animalType;
        IsActive = isActive;
        Position = position;
        Rotation = rotation;
        TargetDirection = targetDirection;
        ChangeDirectionCooldown = changeDirectionCooldown;
        Speed = speed;
        RotationSpeed = rotationSpeed;
        EatDistance = eatDistance;
        Seed = seed;
    }

    public static bool CanEatOtherAnimal(AnimalType animal1, AnimalType animal2)
    {
        switch (animal1)
        {
            case AnimalType.Fox:
                if (animal2 == AnimalType.Bunny) return true;
                break;
            case AnimalType.Bunny:
                if (animal2 == AnimalType.Fox) return false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animal1), animal1, null);
        }

        return false;
    }
}