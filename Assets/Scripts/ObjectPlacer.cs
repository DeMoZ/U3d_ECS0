using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Entities;

//https://www.youtube.com/watch?time_continue=108&v=WZ6-LxwxWEI&feature=emb_logo

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObject _objPrefab;
    [SerializeField] int _objIncrement = 4;

    TransformAccessArray _transforms;
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
            AddObjects(_objIncrement);
        }


//#if UNITY_EDITOR
//        DebugDrowBox(obj.collBox, Color.blue, Time.deltaTime);
//#endif

    }

    private void AddObjects(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(0, 20);
            float yVal = Random.Range(0, 20);
            float zVal = Random.Range(0, 20);

            Vector3 pos = new Vector3(xVal, yVal, zVal);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);

            var obj = Instantiate(_objPrefab, pos, rotation) as GameObject;
            obj.transform.parent = this.transform;
        }

        count += amount;
    }
}

