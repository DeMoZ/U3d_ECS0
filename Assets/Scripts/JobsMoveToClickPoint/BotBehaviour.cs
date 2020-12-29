using System.Collections.Generic;
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

        private Transform[] _transforms;

        /// <summary>
        /// thread safe wrap
        /// </summary>
        private TransformAccessArray _transformAccessArray;

        private List<MovingObject> _movingObjects = new List<MovingObject>();
        private float _secondsCount;

        public BotBehaviour(int numberOfBots, IBotFactory botFactory)
        {
            _numberOfBots = numberOfBots;

            botFactory = new BotFactory();
            _transforms = botFactory.GenerateBots(_numberOfBots);

            InstantiateMovingObjects();
        }

        public void Update()
        {
            MovingObjectsToNative();

            var deltaTime = Time.deltaTime;

            var rotationJob = CreateRotationJob(deltaTime);
            var velocityJob = CreateVelocityJob(deltaTime);
            var moveJob = CreateMoveJob(deltaTime);

            _transformAccessArray = new TransformAccessArray(_transforms); // потокобезопасная обертка
            JobHandle rotationHandle = rotationJob.Schedule(_numberOfBots, 0);
            JobHandle velocityHandle = velocityJob.Schedule(_numberOfBots, 0, rotationHandle);
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, velocityHandle);
            moveHandle.Complete();

            DestroyObject(deltaTime);

            NativeToMovingObjects();
            DisposeNatives();
        }

        private void DisposeNatives()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _rotations.Dispose();
            _transformAccessArray.Dispose();
        }

        private MoveJob CreateMoveJob(float deltaTime)
        {
            return new MoveJob()
            {
                Rotations = _rotations,
                Positions = _positions,
                Velocities = _velocities,
                DeltaTime = deltaTime
            };
        }

        private VelocityJob CreateVelocityJob(float deltaTime)
        {
            return new VelocityJob()
            {
                Velocities = _velocities,
                Positions = _positions,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition
            };
        }

        private RotationJob CreateRotationJob(float deltaTime)
        {
            return new RotationJob()
            {
                Positions = _positions,
                Rotations = _rotations,
                DeltaTime = deltaTime,
                TargetPosition = TargetPosition
            };
        }

        private void MovingObjectsToNative()
        {
            _numberOfBots = _movingObjects.Count;

            _transforms = new Transform[_numberOfBots];
            _rotations = new NativeArray<Quaternion>(_numberOfBots, Allocator.TempJob);
            _positions = new NativeArray<Vector3>(_numberOfBots, Allocator.TempJob);
            _velocities = new NativeArray<Vector3>(_numberOfBots, Allocator.TempJob);

            for (int i = 0; i < _numberOfBots; i++)
            {
                _transforms[i] = _movingObjects[i].Transform;
                _rotations[i] = _movingObjects[i].Transform.rotation;
                _positions[i] = _movingObjects[i].Transform.position;
                _velocities[i] = _movingObjects[i].Velocity;
            }
        }

        private void NativeToMovingObjects()
        {
            _movingObjects = new List<MovingObject>();

            for (int i = 0; i < _transforms.Length; i++)
                if ((object) _transforms[i] != null)
                    _movingObjects.Add(new MovingObject(_transforms[i], _velocities[i]));
        }

        private void InstantiateMovingObjects()
        {
            _movingObjects = new List<MovingObject>();

            for (int i = 0; i < _transforms.Length; i++)
                _movingObjects.Add(new MovingObject(_transforms[i], Vector3.zero));
        }

        private void DestroyObject(float deltaTime)
        {
            _secondsCount += deltaTime;
            if (_secondsCount > 1f)
            {
                _secondsCount = 0;
                var random = Random.Range(0, _transforms.Length);
                GameObject.Destroy(_transforms[random].gameObject);
                _transforms[random] = null;
            }
        }
    }
}