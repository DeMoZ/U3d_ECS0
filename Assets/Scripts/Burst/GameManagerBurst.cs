using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

//https://www.youtube.com/results?search_query="unite+copenhagen",+long
namespace Burst
{
    public class GameManagerBurst : MonoBehaviour
    {
        [SerializeField] GameObject _enemyPrefab;
        [SerializeField] int enemySheepIncrement = 1;

        [SerializeField] float _moveSpeed;
        [SerializeField] float _boundTop;
        [SerializeField] float _boundBottom;
        float _deltatime;

        TransformAccessArray _transforms;
        MovementBurst _movementJob;
        JobHandle _jobHandle;

        [SerializeField] int count;
        private void OnDisable()
        {
            _jobHandle.Complete();
            _transforms.Dispose();
        }

        private void Start()
        {
            _transforms = new TransformAccessArray(0, -1);

        }

        private void Update()
        {
            _jobHandle.Complete();

            if (Input.GetKey("space"))
            {
                AddShips(enemySheepIncrement);
            }

            _movementJob = new MovementBurst
            {
                moveSpeed = _moveSpeed,
                boundTop = _boundTop,
                boundBottom = _boundTop,
                deltatime = Time.deltaTime
            };

            _jobHandle = _movementJob.Schedule(_transforms);
            JobHandle.ScheduleBatchedJobs();
        }

        private void AddShips(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                float xVal = Random.Range(0, 20);
                float yVal = Random.Range(0, 10);

                Vector3 pos = new Vector3(xVal, yVal, 0);
                Quaternion rotation = Quaternion.Euler(0, 180, 0);

                var obj = Instantiate(_enemyPrefab, pos, rotation) as GameObject;
                obj.transform.parent = this.transform;
            }

            count += amount;
        }
    }
}
