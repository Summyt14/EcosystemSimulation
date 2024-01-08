using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalMovementJob : IJobParallelFor
    {
        public NativeArray<Animal> Animals;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int BoundsWidth;
        [ReadOnly] public int BoundsHeight;

        public void Execute(int index)
        {
            Animal animal = Animals[index];

            if (!animal.IsActive) return;

            HandleRandomDirectionChange(ref animal);
            HandleOffBounds(ref animal);
            RotateTowardsTarget(ref animal);
            SetPosition(ref animal);

            Animals[index] = animal;
        }

        private void HandleRandomDirectionChange(ref Animal animal)
        {
            float changeDirectionCooldown = animal.ChangeDirectionCooldown;
            float3 targetDirection = animal.TargetDirection;
            uint seed = animal.Seed;

            changeDirectionCooldown -= DeltaTime;
            if (changeDirectionCooldown <= 0)
            {
                Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
                float angleChange = random.NextFloat(-90f, 90f);
                quaternion newRotation = quaternion.AxisAngle(math.up(), angleChange);
                targetDirection = math.mul(newRotation, targetDirection);
                changeDirectionCooldown = random.NextFloat(1f, 5f);
            }

            animal.ChangeDirectionCooldown = changeDirectionCooldown;
            animal.TargetDirection = targetDirection;
        }

        private void HandleOffBounds(ref Animal animal)
        {
            float3 position = animal.Position;
            float3 targetDirection = animal.TargetDirection;

            if ((position.x < -BoundsWidth && targetDirection.x < 0) || (position.x > BoundsWidth && targetDirection.x > 0))
                targetDirection = new float3(-targetDirection.x, 0, targetDirection.z);

            if ((position.z < -BoundsHeight && targetDirection.z < 0) || (position.z > BoundsHeight && targetDirection.z > 0))
                targetDirection = new float3(targetDirection.x, 0, -targetDirection.z);

            animal.TargetDirection = targetDirection;
        }

        private void RotateTowardsTarget(ref Animal animal)
        {
            quaternion rotation = animal.Rotation;
            float3 targetDirection = animal.TargetDirection;
            float rotationSpeed = animal.RotationSpeed;

            quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
            rotation = math.slerp(rotation, targetRotation, rotationSpeed * DeltaTime);

            animal.Rotation = rotation;
        }

        private void SetPosition(ref Animal animal)
        {
            float3 position = animal.Position;
            quaternion rotation = animal.Rotation;
            float speed = animal.Speed;

            float3 newPosition = math.mul(rotation, math.forward() * speed * DeltaTime);
            position += newPosition;

            animal.Position = position;
        }
    }
}