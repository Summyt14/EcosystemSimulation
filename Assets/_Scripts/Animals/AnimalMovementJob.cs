using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalMovementJob : IJobParallelFor
    {
        public NativeArray<AnimalData> AnimalDataNativeArray;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int BoundsWidth;
        [ReadOnly] public int BoundsHeight;
        [ReadOnly] public int Seed;

        public void Execute(int index)
        {
            AnimalData animalData = AnimalDataNativeArray[index];
            if (!animalData.IsActive) return;
            Random randomGen = new((uint)((index + 1) * Seed) + 1);
            float rand = randomGen.NextFloat(0, 100);

            HandleRandomDirectionChange(ref animalData, rand);
            HandleOffBounds(ref animalData);
            RotateTowardsTarget(ref animalData);
            SetPosition(ref animalData);
            
            AnimalDataNativeArray[index] = animalData;
        }

        private void HandleRandomDirectionChange(ref AnimalData animalData, float randomFloat)
        {
            float changeDirectionCooldown = animalData.ChangeDirectionCooldown;
            float3 targetDirection = animalData.TargetDirection;

            changeDirectionCooldown -= DeltaTime;
            if (changeDirectionCooldown <= 0)
            {
                float angleChange = math.remap(0, 100f, -90f, 90f, randomFloat);
                quaternion newRotation = quaternion.AxisAngle(math.up(), angleChange);
                targetDirection = math.mul(newRotation, targetDirection);
                changeDirectionCooldown = math.remap(0, 100f, 1f, 5f, randomFloat);
            }

            animalData.ChangeDirectionCooldown = changeDirectionCooldown;
            animalData.TargetDirection = targetDirection;
        }

        private void HandleOffBounds(ref AnimalData animalData)
        {
            float3 position = animalData.Position;
            float3 targetDirection = animalData.TargetDirection;

            if ((position.x < -BoundsWidth && targetDirection.x < 0) || (position.x > BoundsWidth && targetDirection.x > 0))
                targetDirection = new float3(-targetDirection.x, 0, targetDirection.z);

            if ((position.z < -BoundsHeight && targetDirection.z < 0) || (position.z > BoundsHeight && targetDirection.z > 0))
                targetDirection = new float3(targetDirection.x, 0, -targetDirection.z);

            animalData.TargetDirection = targetDirection;
        }

        private void RotateTowardsTarget(ref AnimalData animalData)
        {
            quaternion rotation = animalData.Rotation;
            quaternion targetRotation = quaternion.LookRotation(animalData.TargetDirection, math.up());
            rotation = math.slerp(rotation, targetRotation, animalData.RotationSpeed * DeltaTime);

            animalData.Rotation = rotation;
        }

        private void SetPosition(ref AnimalData animalData)
        {
            float3 position = animalData.Position;
            float3 newPosition = math.mul(animalData.Rotation, math.forward() * animalData.Speed * DeltaTime);
            position += newPosition;

            animalData.Position = position;
        }
    }
}