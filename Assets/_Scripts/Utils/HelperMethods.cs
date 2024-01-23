using System;
using System.Collections.Generic;
using System.IO;
using _Scripts.AliveObjects;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class HelperMethods
    {
        public static float Map(float x, float x1, float x2, float y1, float y2)
        {
            float m = (y2 - y1) / (x2 - x1);
            float c = y1 - m * x1;
            return m * x + c;
        }

        public static T ReadFromJson<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        public static void ExportToJson<T>(T obj, string path)
        {
            string json = JsonUtility.ToJson(obj, true);
            File.WriteAllText(path, json);
        }
    }

    [Serializable]
    public class WrapperHolder
    {
        public List<AnimalDataWrapper> list;

        public WrapperHolder() => list = new List<AnimalDataWrapper>();
        public WrapperHolder(List<AnimalDataWrapper> list) => this.list = list;
    }

    [Serializable]
    public class AnimalDataWrapper
    {
        public List<AnimalData> animals;
        public float timeSinceStart;
        public int foxCount;
        public int chickenCount;
        public int grassCount;

        public AnimalDataWrapper(List<AnimalData> animals, float timeSinceStart, Dictionary<AliveObjectSo.Type, int> animalCount)
        {
            this.animals = animals;
            this.timeSinceStart = timeSinceStart;
            foxCount = animalCount[AliveObjectSo.Type.Fox];
            chickenCount = animalCount[AliveObjectSo.Type.Chicken];
            grassCount = animalCount[AliveObjectSo.Type.Grass];
        }
    }
}