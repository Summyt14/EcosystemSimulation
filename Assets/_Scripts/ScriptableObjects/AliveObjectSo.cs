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
                    if (animal2 == AliveObjectSo.Type.Chicken) return true;
                    break;
                case AliveObjectSo.Type.Chicken:
                    if (animal2 == AliveObjectSo.Type.Fox) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animal1), animal1, "Unknown animal type");
            }

            return false;
        }
    }
}