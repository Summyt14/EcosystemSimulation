using System;
using _Scripts.AliveObjects;
using _Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class StatsUI : MonoBehaviour
    {
        [Serializable]
        public struct TypeCount
        {
            public AliveObjectSo.Type type;
            public TextMeshProUGUI textCount;
        }

        [SerializeField] private TypeCount[] typeCountList;

        private void Start()
        {
            AnimalManager.Instance.OnAliveObjectCountChanged += OnAliveObjectCountChanged;
        }

        private void OnDestroy()
        {
            AnimalManager.Instance.OnAliveObjectCountChanged -= OnAliveObjectCountChanged;
        }

        private void OnAliveObjectCountChanged()
        {
            foreach (TypeCount typeCount in typeCountList)
            {
                if (AnimalManager.Instance.AliveObjectCount.TryGetValue(typeCount.type, out int count))
                    typeCount.textCount.text = $"{typeCount.type} : {count.ToString()}";
            }
        }
    }
}