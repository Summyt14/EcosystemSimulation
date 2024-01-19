using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/Animal")]
    public class AnimalSo : AliveObjectSo
    {
        public float initialRotationSpeed = 2f;
    }
}