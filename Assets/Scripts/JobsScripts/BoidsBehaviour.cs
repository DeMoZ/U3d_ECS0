using System.ComponentModel;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace JobsScripts
{
    public class BoidsBehaviour : MonoBehaviour
    {
        [SerializeField] private int _numberOfEntities;
        [SerializeField] private GameObject _entityPrefab;
        [SerializeField] private float _destinationThreshold;
        [SerializeField] private float _velocityLimit;

        [Tooltip("GroundHeight")] private float _groundHeight = 0;

        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _accelerations;

        private TransformAccessArray _transformAccessArray; // структура принимает в конструктор массив трансформов

        private void Start()
        {
            _positions = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _accelerations = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            var transforms = new Transform[_numberOfEntities];

            for (int i = 0; i < _numberOfEntities; i++)
            {
                transforms[i] = Instantiate(_entityPrefab).transform;
                var random = Random.insideUnitCircle;
                _velocities[i] = new Vector3(random.x, _groundHeight, random.y);
            }

            _transformAccessArray = new TransformAccessArray(transforms); // потокобезопасная обертка
        }

        private void Update()
        {
            var accelerationJob = new AccelerationJob()
            {
                Accelerations = _accelerations,
                Positions = _positions,
                Velocities = _velocities,
                DestinationThreshold = _destinationThreshold
            };
            var moveJob = new MoveJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                Accelerations = _accelerations,
                DeltaTime = Time.deltaTime,
                VelocityLimit = _velocityLimit
            };

            JobHandle accelerationHandle = accelerationJob.Schedule( _numberOfEntities, 0);
            JobHandle moveHandle = moveJob.Schedule(_transformAccessArray,accelerationHandle);
            //accelerationHandle.Complete();
            moveHandle.Complete(); // основной поток подождет выполнения этой задачи
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _accelerations.Dispose();
            _transformAccessArray.Dispose();
        }
    }
}