using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/Animal")]
    public class AnimalSo : AliveObjectSo
    {
        public float initialSpeed = 5f;
        public float initialRotationSpeed = 2f;
        public float initialEatDistance = 1f;
        public float hungerDecayRate = 0.1f;
    }
}