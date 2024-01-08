using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Animals
{
    public class AnimalJobMove : MonoBehaviour
    {
        [SerializeField] private int boundsWidth;
        [SerializeField] private int boundsHeight;

        private Camera _camera;
        private float3 _targetDirection;
        private float _changeDirectionCooldown;
        private float _speed;
        private float _rotationSpeed;

        private NativeArray<float> _changeDirectionCooldownResult;
        private NativeArray<float3> _targetDirectionResult;
        private NativeArray<float3> _positionResult;
        private NativeArray<quaternion> _rotationResult;

        private JobHandle _jobHandle;

        private void Awake()
        {
            _camera = Camera.main;
            _targetDirection = new float3(0f, 0f, 1f);
            _speed = Random.Range(2f, 5f);
            _rotationSpeed = Random.Range(90f, 180f);

            _changeDirectionCooldownResult = new NativeArray<float>(1, Allocator.Persistent);
            _targetDirectionResult = new NativeArray<float3>(1, Allocator.Persistent);
            _positionResult = new NativeArray<float3>(1, Allocator.Persistent);
            _rotationResult = new NativeArray<quaternion>(1, Allocator.Persistent);
        }

        private void Update()
        {
            AnimalJob job = new(transform.position, transform.rotation, _targetDirection,
                _changeDirectionCooldown, _speed, _rotationSpeed, Time.deltaTime,
                (uint)Random.Range(1, 1000), boundsWidth, boundsHeight, _changeDirectionCooldownResult,
                _targetDirectionResult, _positionResult, _rotationResult);
            _jobHandle = job.Schedule();
        }

        private void LateUpdate()
        {
            _jobHandle.Complete();

            _changeDirectionCooldown = _changeDirectionCooldownResult[0];
            _targetDirection = _targetDirectionResult[0];
            transform.rotation = _rotationResult[0];
            transform.position = _positionResult[0];
        }

        private void OnDestroy()
        {
            _changeDirectionCooldownResult.Dispose();
            _targetDirectionResult.Dispose();
            _positionResult.Dispose();
            _rotationResult.Dispose();
        }
    }
}