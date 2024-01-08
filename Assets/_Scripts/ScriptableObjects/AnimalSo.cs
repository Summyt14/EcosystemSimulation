using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimalSo", menuName = "ScriptableObjects/AnimalSo")]
    public class AnimalSo : ScriptableObject
    {
        public GameObject prefab;
        public int initialCount = 10;
        public float initialSize = 1f;
        public float initialSpeed = 5f;
        public float initialRotationSpeed = 2f;
        public float initialEatDistance = 1f;
        public Animal.AnimalType type;
    }
}