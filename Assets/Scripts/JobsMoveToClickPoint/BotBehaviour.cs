using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    public class BotBehaviour
    {
        private int _numberOfBots;

        public Vector3 TargetPosition;

        private NativeArray<Vector3> _positions;
        private NativeArray<Quaternion> _rotations;
        private NativeArray<Vector3> _velocities;
        private NativeArray<int> _exists;

        private Transform[] _transforms;


        /// <summary>
        /// thread safe wrap
        /// </summary>
        private TransformAccessArray _transformAccessArray;

        private float _secondsCount;

        private bool _reinitialize;

        private int _countedBotsAmount;

        public BotBehaviour(int numberOfBots, IBotFactory botFactory)
        {
            _numberOfBots = numberOfBots;

            botFactory = new BotFactory();

            _transforms = botFactory.GenerateBots(_numberOfBots);
            _positions = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);
            _rotations = new NativeArray<Quaternion>(_numberOfBots, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);
            _exists = new NativeArray<int>(_numberOfBots, Allocator.Persistent);
            InitializeExists();

            _transformAccessArray = new TransformAccessArray(_transforms); // потокобезопасная обертка
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;

            var rotationJob = CreateRotationJob(deltaTime);
            var velocityJob = CreateVelocityJob(deltaTime);
            var moveJob = CreateMoveJob(deltaTime);

            JobHandle rotationHandle = rotationJob.Schedule(_numberOfBots, 0);
            JobHandle velocityHandle = velocityJob.Schedule(_numberOfBots, 0, rotationHandle);
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, velocityHandle);
            moveHandle.Complete();

            DestroyObject(deltaTime);

            ReduceNativeArrays();
        }

        private void ReduceNativeArrays()
        {
            if (_reinitialize)
            {
                var countedNumberOfBots = new NativeArray<int>(1, Allocator.TempJob);

                CountNewArrayDimension(countedNumberOfBots);
                // после посчета, надо переинициализировать

                Reinitialize();
            }
        }

        private void Reinitialize()
        {
// знаю количество старых и количество новых массивов
            // var transforms = new Transform[_countedBotsAmount];
            // var positions = new NativeArray<Vector3>(_numberOfBots, Allocator.TempJob);
            // var rotations = new NativeArray<Quaternion>(_numberOfBots, Allocator.TempJob);
            // var velocities = new NativeArray<Vector3>(_numberOfBots, Allocator.TempJob);
            // var exists = new NativeArray<int>(_numberOfBots, Allocator.TempJob);

            SwapArrays swapArraysJob = new SwapArrays()
            {
                Positions = _positions,
                Rotations = _rotations,
                Velocities = _velocities,
                Exists = _exists,
                CountedBotsAmount = _countedBotsAmount
            };

            var swapArraysHandle = swapArraysJob.Schedule(_transformAccessArray);
            swapArraysHandle.Complete();
        }

        public void OnDestroy()
        {
            DisposeNatives();
        }

        private void DisposeNatives()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _rotations.Dispose();
            _transformAccessArray.Dispose();
        }

        private void CountNewArrayDimension(NativeArray<int> countedNumberOfBots)
        {
            var сountExistsJob = new CountExistsJob()
            {
                NumberOfBots = countedNumberOfBots,
                Exists = _exists
            };

            JobHandle reinitializeHandle = сountExistsJob.Schedule();
            reinitializeHandle.Complete();

            _countedBotsAmount = countedNumberOfBots[0];
            countedNumberOfBots.Dispose();
        }

        private VelocityJob CreateVelocityJob(float deltaTime)
        {
            return new VelocityJob()
            {
                Velocities = _velocities,
                Positions = _positions,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition,
                Exists = _exists
            };
        }

        private RotationJob CreateRotationJob(float deltaTime)
        {
            return new RotationJob()
            {
                Positions = _positions,
                Rotations = _rotations,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition,
                Exists = _exists
            };
        }

        private MoveJob CreateMoveJob(float deltaTime)
        {
            return new MoveJob()
            {
                Rotations = _rotations,
                Positions = _positions,
                Velocities = _velocities,
                DeltaTime = deltaTime,
                Exists = _exists
            };
        }

        private void DestroyObject(float deltaTime)
        {
            _secondsCount += deltaTime;
            if (_secondsCount > 1f)
            {
                _secondsCount = 0;

                _reinitialize = true;

                var random = Random.Range(0, _transforms.Length);

                if (!_transforms[random]) return;

                GameObject.Destroy(_transforms[random].gameObject);
                _transforms[random] = null;
                _exists[random] = 0;
            }
        }

        private void InitializeExists()
        {
            var initializeExistsJob = new InitializeExistsJob {Exists = _exists};

            var initializeExistsHandle = initializeExistsJob.Schedule(_numberOfBots, 0);
            initializeExistsHandle.Complete();
        }
    }

    [BurstCompile]
    public struct SwapArrays : IJobParallelForTransform
    {
        public NativeArray<Quaternion> Rotations;
        public NativeArray<Vector3> Velocities;
        public NativeArray<int> Exists;
        public int CountedBotsAmount;
        public NativeArray<Vector3> Positions;

        public void Execute(int index, TransformAccess transform)
        {
        }
    }
}