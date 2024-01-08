using Unity.Burst;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace _Scripts.Animals
{
    [BurstCompile]
    public struct AnimalJob : IJob
    {
        private float3 _position;
        private quaternion _rotation;
        private float3 _targetDirection;
        private float _changeDirectionCooldown;
        [ReadOnly] private float _speed;
        [ReadOnly] private float _rotationSpeed;
        [ReadOnly] private float _deltaTime;
        [ReadOnly] private uint _seed;
        [ReadOnly] private int _boundsWidth;
        [ReadOnly] private int _boundsHeight;
        [WriteOnly] private NativeArray<float> _changeDirectionCooldownResult;
        [WriteOnly] private NativeArray<float3> _targetDirectionResult;
        [WriteOnly] private NativeArray<float3> _positionResult;
        [WriteOnly] private NativeArray<quaternion> _rotationResult;


        public AnimalJob(float3 position, quaternion rotation, float3 targetDirection, float changeDirectionCooldown, float speed,
            float rotationSpeed, float deltaTime, uint seed, int boundsWidth, int boundsHeight, NativeArray<float> changeDirectionCooldownResult,
            NativeArray<float3> targetDirectionResult, NativeArray<float3> positionResult, NativeArray<quaternion> rotationResult)
        {
            _position = position;
            _rotation = rotation;
            _targetDirection = targetDirection;
            _changeDirectionCooldown = changeDirectionCooldown;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _deltaTime = deltaTime;
            _seed = seed;
            _boundsWidth = boundsWidth;
            _boundsHeight = boundsHeight;
            _changeDirectionCooldownResult = changeDirectionCooldownResult;
            _targetDirectionResult = targetDirectionResult;
            _positionResult = positionResult;
            _rotationResult = rotationResult;
        }

        public void Execute()
        {
            UpdateTargetDirection();
            RotateTowardsTarget();
            SetPosition();
        }

        private void UpdateTargetDirection()
        {
            HandleRandomDirectionChange();
            HandleOffBounds();
            _targetDirectionResult[0] = _targetDirection;
        }

        private void HandleRandomDirectionChange()
        {
            _changeDirectionCooldown -= _deltaTime;
            if (_changeDirectionCooldown <= 0)
            {
                Unity.Mathematics.Random random = new(_seed);
                float angleChange = random.NextFloat(-90f, 90f);
                quaternion newRotation = quaternion.AxisAngle(math.up(), angleChange);
                _targetDirection = math.mul(newRotation, _targetDirection);
                _changeDirectionCooldown = random.NextFloat(1f, 5f);
            }

            _changeDirectionCooldownResult[0] = _changeDirectionCooldown;
        }

        private void HandleOffBounds()
        {
            if ((_position.x < -_boundsWidth && _targetDirection.x < 0) ||
                (_position.x > _boundsWidth && _targetDirection.x > 0))
            {
                _targetDirection = new Vector3(-_targetDirection.x, 0, _targetDirection.z);
            }

            if ((_position.z < -_boundsHeight && _targetDirection.z < 0) ||
                (_position.z > _boundsHeight && _targetDirection.z > 0))
            {
                _targetDirection = new Vector3(_targetDirection.x, 0, -_targetDirection.z);
            }
        }

        private void RotateTowardsTarget()
        {
            quaternion targetRotation = quaternion.LookRotation(_targetDirection, math.up());
            _rotation = math.slerp(_rotation, targetRotation, _rotationSpeed * _deltaTime);
            _rotationResult[0] = _rotation;
        }

        private void SetPosition()
        {
            float3 newPosition = math.mul(_rotation, math.forward() * _speed * _deltaTime);
            _position += newPosition;
            _positionResult[0] = _position;
        }
    }
}