using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    public class BotBehaviour
    {
        private readonly int _numberOfBots;

        public Vector3 TargetPosition;

        private NativeArray<Vector3> _positions;
        private NativeArray<Quaternion> _rotations;
        private NativeArray<Vector3> _velocities;

        /// <summary>
        /// thread safe wrap
        /// </summary>
        private TransformAccessArray _transformAccessArray;

        public BotBehaviour(int numberOfBots, IBotFactory botFactory)
        {
            _numberOfBots = numberOfBots;

            var transforms = new Transform[_numberOfBots];
            _rotations = new NativeArray<Quaternion>(_numberOfBots, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);
            _positions = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);

            botFactory = new BotFactory();
            transforms = botFactory.GenerateBots(_numberOfBots, ref _rotations, ref _positions);

            _transformAccessArray = new TransformAccessArray(transforms); // потокобезопасная обертка
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;
            var rotationJob = new RotationJob()
            {
                Positions = _positions,
                Rotations = _rotations,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition
            };

            var velocityJob = new VelocityJob()
            {
                Velocities = _velocities,
                Positions = _positions,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition
            };

            var moveJob = new MoveJob()
            {
                Rotations = _rotations,
                Positions = _positions,
                Velocities = _velocities,
                DeltaTime = deltaTime
            };

            JobHandle rotationHandle = rotationJob.Schedule(_numberOfBots, 0);
            JobHandle velocityHandle = velocityJob.Schedule(_numberOfBots, 0, rotationHandle);
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, velocityHandle);

            moveHandle.Complete();
        }

        public void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _rotations.Dispose();
            _transformAccessArray.Dispose();
        }
    }
}