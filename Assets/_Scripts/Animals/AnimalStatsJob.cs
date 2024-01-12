using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalStatsJob : IJobParallelFor
    {
        public NativeArray<AnimalData> AnimalDataNativeArray;
        public NativeArray<AnimalData> NewAnimalNativeArray;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int Seed;

        public void Execute(int index)
        {
            AnimalData animalData = AnimalDataNativeArray[index];
            if (!animalData.IsActive) return;
            Unity.Mathematics.Random randomGen = new((uint)((index + 1) * Seed) + 1);
            float rand = randomGen.NextFloat(0, 100);

            HandleEatingOtherAnimals(ref animalData, index);
            HandleStats(ref animalData);
            TryReproduce(ref animalData, rand, index);

            AnimalDataNativeArray[index] = animalData;
        }

        private void HandleEatingOtherAnimals(ref AnimalData animalData, int index)
        {
            NativeArray<AnimalData> updatedAnimalDataArray = new(AnimalDataNativeArray.Length, Allocator.Temp);
            AnimalDataNativeArray.CopyTo(updatedAnimalDataArray);

            for (int i = 0; i < AnimalDataNativeArray.Length; i++)
            {
                // Skip self and inactive animals in array
                if (i == index) continue;
                AnimalData otherAnimalData = updatedAnimalDataArray[i];
                if (!otherAnimalData.IsActive) continue;

                // Check if animal can eat other animal and if so eat it and update hunger
                float distance = math.distance(animalData.Position, otherAnimalData.Position);
                if (distance <= animalData.EatDistance && animalData.Type.CanEat(otherAnimalData.Type))
                {
                    otherAnimalData.IsActive = false;
                    animalData.Hunger += otherAnimalData.HungerIncreaseWhenEaten;
                    updatedAnimalDataArray[i] = otherAnimalData;
                }
            }

            // Update array
            updatedAnimalDataArray.CopyTo(AnimalDataNativeArray);
            updatedAnimalDataArray.Dispose();
        }

        private void HandleStats(ref AnimalData animalData)
        {
            // Check if animal is fully hungry and if so deactivate it
            if (animalData.Hunger <= 0)
            {
                animalData.IsActive = false;
                return;
            }

            // Update stats based on time since last update and decrease hunger 
            animalData.TimeAlive += DeltaTime;
            animalData.Hunger -= math.clamp(animalData.HungerDecayRate * DeltaTime, 0, 100);
        }

        private void TryReproduce(ref AnimalData animalData, float randomFloat, int index)
        {
            if (!animalData.IsActive) return;
            // Check if animal can reproduce
            animalData.ReproduceCooldown += DeltaTime;
            if (animalData.ReproduceCooldown < animalData.TryReproduceRate) return;
            animalData.ReproduceCooldown = 0;
            float chance = math.clamp(randomFloat, 0, 100);
            // Reproduce with 10% chance
            if (chance >= 90f)
            {
                AnimalData newAnimalData = animalData.Copy();
                NewAnimalNativeArray[index] = newAnimalData;
            }
        }
    }
}