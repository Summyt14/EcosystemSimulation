using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalMovementJob : IJobParallelFor
    {
        public NativeArray<AnimalData> Animals;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int BoundsWidth;
        [ReadOnly] public int BoundsHeight;

        public void Execute(int index)
        {
            AnimalData animalData = Animals[index];
            if (!animalData.IsActive) return;

            HandleRandomDirectionChange(ref animalData);
            HandleOffBounds(ref animalData);
            RotateTowardsTarget(ref animalData);
            SetPosition(ref animalData);

            Animals[index] = animalData;
        }

        private void HandleRandomDirectionChange(ref AnimalData animalData)
        {
            float changeDirectionCooldown = animalData.ChangeDirectionCooldown;
            float3 targetDirection = animalData.TargetDirection;

            changeDirectionCooldown -= DeltaTime;
            if (changeDirectionCooldown <= 0)
            {
                Random random = new(animalData.Seed);
                float angleChange = random.NextFloat(-90f, 90f);
                quaternion newRotation = quaternion.AxisAngle(math.up(), angleChange);
                targetDirection = math.mul(newRotation, targetDirection);
                changeDirectionCooldown = random.NextFloat(1f, 5f);
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