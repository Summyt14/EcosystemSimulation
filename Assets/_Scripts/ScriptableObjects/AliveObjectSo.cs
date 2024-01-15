using System;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Alive Object", menuName = "Scriptable Objects/Alive Object")]
    public class AliveObjectSo : ScriptableObject
    {
        public enum Type
        {
            Fox = 0,
            Chicken = 1,
            Grass = 3
        }

        public GameObject prefab;
        public Type type;
        public int initialCount = 10;
        public int hungerIncreaseWhenEaten = 50;
        public float tryReproduceRate = 1f;
    }

    public static class TypeExtensions
    {
        public static bool CanEat(this AliveObjectSo.Type animal1, AliveObjectSo.Type animal2)
        {
            switch (animal1)
            {
                case AliveObjectSo.Type.Fox:
                    return animal2 switch
                    {
                        AliveObjectSo.Type.Fox => false,
                        AliveObjectSo.Type.Chicken => true,
                        AliveObjectSo.Type.Grass => false,
                        _ => throw new ArgumentOutOfRangeException(nameof(animal2), animal2, "Unknown animal type")
                    };

                case AliveObjectSo.Type.Chicken:
                    return animal2 switch
                    {
                        AliveObjectSo.Type.Fox => false,
                        AliveObjectSo.Type.Grass => true,
                        AliveObjectSo.Type.Chicken => false,
                        _ => throw new ArgumentOutOfRangeException(nameof(animal2), animal2, "Unknown animal type")
                    };

                case AliveObjectSo.Type.Grass:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(animal1), animal1, "Unknown animal type");
            }

            return false;
        }
    }
}