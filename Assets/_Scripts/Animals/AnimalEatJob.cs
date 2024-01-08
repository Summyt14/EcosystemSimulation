using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalEatJob : IJobParallelFor
    {
        public NativeArray<Animal> Animals;

        public void Execute(int index)
        {
            Animal animal = Animals[index];
            if (!animal.IsActive) return;

            for (int i = 0; i < Animals.Length; i++)
            {
                if (i == index) continue;
                Animal otherAnimal = Animals[i];
                if (!otherAnimal.IsActive) continue;

                float distance = math.distance(animal.Position, otherAnimal.Position);
                if (distance <= 1f && Animal.CanEatOtherAnimal(animal.Type, otherAnimal.Type))
                {
                    otherAnimal.IsActive = false;
                    Animals[i] = otherAnimal;
                }
            }
        }
    }
}