using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace JobsScripts
{
    public class BoidsBehaviour : MonoBehaviour
    {
        [SerializeField] private int _numberOfEntities;
        [SerializeField] private GameObject _entityPrefab;

        [Tooltip("GroundHeight")]
        private float _groundHeight = 0;
        
        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;

        private TransformAccessArray _transformAccessArray; // структура принимает в конструктор массив трансформов

        private void Start()
        {
            _positions = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);

            var transforms = new Transform[_numberOfEntities];

            for (int i = 0; i < _numberOfEntities; i++)
            {
                transforms[i] = Instantiate(_entityPrefab).transform;
                var random=Random.insideUnitCircle;
                _velocities[i] = new Vector3(random.x, _groundHeight, random.y);
            }

            _transformAccessArray = new TransformAccessArray(transforms); // потокобезопасная обертка
        }

        private void Update()
        {
            var moveJob = new MoveJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                DeltaTime = Time.deltaTime
            };

            var jobHandle = moveJob.Schedule(_transformAccessArray);    
            jobHandle.Complete();       // основной поток подождет выполнения этой задачи
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velocities.Dispose();
            _transformAccessArray.Dispose();
        }
    }
}