using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int _numberOfBots;

        private NativeArray<Vector3> _positions;

        /// <summary>
        /// bots rotations
        /// </summary>
        private NativeArray<Quaternion> _rotations;

        /// <summary>
        /// bots move directions
        /// </summary>
        private NativeArray<Vector3> _velocities;

        /// <summary>
        /// thread safe wrap
        /// </summary>
        private TransformAccessArray _transformAccessArray;

        private Vector3 _targetPosition;

        private void Start()
        {
            var transforms = new Transform[_numberOfBots];
            _rotations = new NativeArray<Quaternion>(_numberOfBots, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);
            _positions = new NativeArray<Vector3>(_numberOfBots, Allocator.Persistent);

            Clicker clicker = new Clicker(SetTargetPoint);


            IBotFactory botFactory = new BotFactory();
            transforms = botFactory.GenerateBots(_numberOfBots, ref _rotations, ref _velocities, ref _positions);


            _transformAccessArray = new TransformAccessArray(transforms); // потокобезопасная обертка
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
             var rotationJob = new RotationJob()
             {
                 Positions = _positions,
                 Rotations = _rotations,
                 DeltaTime = deltaTime,
                 TargetPosition = _targetPosition
             };

            var velocityJob = new VelocityJob()
            {
                Velocities = _velocities,
                Positions = _positions,
                DeltaTime = deltaTime,
                TargetPosition = _targetPosition
            };

            var moveJob = new MoveJob()
            {
                Rotations = _rotations,
                Positions = _positions,
                Velocities = _velocities,
                DeltaTime = deltaTime
            };

            JobHandle rotationHandle = rotationJob.Schedule(_numberOfBots,0);
            JobHandle velocityHandle = velocityJob.Schedule(_numberOfBots,0,rotationHandle);
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, velocityHandle);

            moveHandle.Complete();
        }

        private void SetTargetPoint(Vector3 point) => 
            _targetPosition = point;

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _rotations.Dispose();
            _transformAccessArray.Dispose();
        }
    }
}