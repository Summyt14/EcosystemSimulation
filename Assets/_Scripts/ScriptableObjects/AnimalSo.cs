using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/Animal")]
    public class AnimalSo : AliveObjectSo
    {
        public float reproduceChance = 1f;
        public float mutationChance = 1f;
        public float initialRotationSpeed = 2f;
    }
}