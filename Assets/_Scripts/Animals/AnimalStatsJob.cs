using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalStatsJob : IJobParallelFor
    {
        public NativeArray<AnimalData> Animals;
        [ReadOnly] public float DeltaTime;

        public void Execute(int index)
        {
            AnimalData animalData = Animals[index];
            if (!animalData.IsActive) return;
            
            HandleEatingOtherAnimals(ref animalData, index);
            HandleStats(ref animalData);
            TryReproduce(ref animalData);
            
            Animals[index] = animalData;
        }

        private void HandleEatingOtherAnimals(ref AnimalData animalData, int index)
        {
            NativeArray<AnimalData> updatedAnimals = new(Animals.Length, Allocator.Temp);
            Animals.CopyTo(updatedAnimals);
            
            for (int i = 0; i < Animals.Length; i++)
            {
                if (i == index) continue;
                AnimalData otherAnimalData = updatedAnimals[i];
                if (!otherAnimalData.IsActive) continue;

                float distance = math.distance(animalData.Position, otherAnimalData.Position);
                if (distance <= animalData.EatDistance && animalData.Type.CanEat(otherAnimalData.Type))
                {
                    otherAnimalData.IsActive = false;
                    animalData.Hunger += otherAnimalData.HungerIncreaseWhenEaten;
                    updatedAnimals[i] = otherAnimalData;
                }
            }
            
            updatedAnimals.CopyTo(Animals);
            updatedAnimals.Dispose();
        }

        private void HandleStats(ref AnimalData animalData)
        {
            if (animalData.Hunger <= 0)
            {
                animalData.IsActive = false;
                return;
            }
            animalData.TimeAlive += DeltaTime;
            animalData.Hunger -= math.max(0, animalData.HungerDecayRate * DeltaTime);
        }

        private void TryReproduce(ref AnimalData animalData)
        {
            Random random = new(animalData.Seed);
            if (random.NextFloat(0f, 100f) <= 1f)
            {
                // being spammed because same seed?
                // NativeArray<AnimalData> updatedAnimals = new(Animals.Length + 1, Allocator.Temp);
                // Animals.CopyTo(updatedAnimals);
                // AnimalData newAnimal = animalData.Copy();
                // updatedAnimals[Animals.Length] = newAnimal;
                // updatedAnimals.CopyTo(Animals);
                // updatedAnimals.Dispose();
            }
        }
    }
}