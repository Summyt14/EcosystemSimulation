using UnityEngine;

namespace _Scripts.Animals.Behaviors
{
    public class RandomMoveBehavior
    {
        private float _deltaTime;
        private readonly int _boundsWidth;
        private readonly int _boundsHeight;
        
        public RandomMoveBehavior(int boundsWidth, int boundsHeight)
        {
            _boundsWidth = boundsWidth;
            _boundsHeight = boundsHeight;
        }

        public void Update(ref AnimalData animalData, float deltaTime)
        {
            _deltaTime = deltaTime;
            if (!animalData.IsActive) return;

            HandleRandomDirectionChange(ref animalData);
            HandleOffBounds(ref animalData);
            RotateTowardsTarget(ref animalData);
            SetPosition(ref animalData);
        }

        private void HandleRandomDirectionChange(ref AnimalData animalData)
        {
            float changeDirectionCooldown = animalData.ChangeDirectionCooldown;
            Vector3 targetDirection = animalData.TargetDirection;

            changeDirectionCooldown -= _deltaTime;
            if (changeDirectionCooldown <= 0)
            {
                float angleChange = Random.Range(-90f, 90f);
                Quaternion newRotation = Quaternion.AngleAxis(angleChange, Vector3.up);
                targetDirection = newRotation * targetDirection;
                changeDirectionCooldown = Random.Range(1f, 5f);
            }

            animalData.ChangeDirectionCooldown = changeDirectionCooldown;
            animalData.TargetDirection = targetDirection;
        }

        private void HandleOffBounds(ref AnimalData animalData)
        {
            Vector3 position = animalData.Position;
            Vector3 targetDirection = animalData.TargetDirection;

            if ((position.x < -_boundsWidth && targetDirection.x < 0) || (position.x > _boundsWidth && targetDirection.x > 0))
                targetDirection = new Vector3(-targetDirection.x, 0, targetDirection.z);

            if ((position.z < -_boundsHeight && targetDirection.z < 0) || (position.z > _boundsHeight && targetDirection.z > 0))
                targetDirection = new Vector3(targetDirection.x, 0, -targetDirection.z);

            animalData.TargetDirection = targetDirection;
        }

        private void RotateTowardsTarget(ref AnimalData animalData)
        {
            Quaternion rotation = animalData.Rotation;
            Quaternion targetRotation = Quaternion.LookRotation(animalData.TargetDirection, Vector3.up);
            rotation = Quaternion.Slerp(rotation, targetRotation, animalData.RotationSpeed * _deltaTime);

            animalData.Rotation = rotation;
        }

        private void SetPosition(ref AnimalData animalData)
        {
            Vector3 position = animalData.Position;
            Vector3 newPosition = animalData.Rotation * Vector3.forward * (animalData.Speed * _deltaTime);
            position += newPosition;

            animalData.Position = position;
        }
    }
}