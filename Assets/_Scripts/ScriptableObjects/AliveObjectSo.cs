using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        public Type[] canEatList;
        public int initialCount = 10;
        public int hungerDecreaseWhenEaten = 50;
        public float tryReproduceRate = 1f;
    }
}