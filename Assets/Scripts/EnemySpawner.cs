using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    private Entity _enemyEntityPrefab;
    private Entity GetEnemyEntityPrefb
    {
        get
        {
            if (_enemyEntityPrefab == null)
            {
                Debug.Log("Prefab not exist, Creating");
                var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
                _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_enemyPrefab, settings);
            }

            return _enemyEntityPrefab;
        }
    }

    [SerializeField] private int _spawnCount = 100;
    [SerializeField] private float _spawnRadius = 10f;

    [Range(1, 10)]
    [SerializeField] private float2 _speed;

    [SerializeField] private MainUI _mainUI;

    private EntityManager entityManager;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        // перевод префаба в сущность
        _enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_enemyPrefab, settings);

        //  PlusEntities();
    }




    private float3 RandomPointOnCircle(object spawnRadius)
    {
        return new float3(UnityEngine.Random.Range(-10, 10),
                          1f,
                          UnityEngine.Random.Range(-10, 10));
    }

    private Quaternion RandomRotation()
    {
        float3 euler = new float3(0, UnityEngine.Random.Range(-180, 180), 0);

        return Quaternion.Euler(euler);
    }

    public void PlusEntities()
    {
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(_spawnCount, Allocator.Temp);

        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(GetEnemyEntityPrefb);
            entityManager.SetComponentData(enemyArray[i], new Translation { Value = RandomPointOnCircle(_spawnRadius) });
            entityManager.SetComponentData(enemyArray[i], new Rotation { Value = RandomRotation() });
            entityManager.SetComponentData(enemyArray[i], new TurnTimer { TimeRange = new float2(1, 8) });
        }

        enemyArray = entityManager.GetAllEntities(Allocator.Temp);
        _mainUI.SetTotalEnemyValue(enemyArray.Length);
        enemyArray.Dispose();
    }

    public void MinusEntities()
    {
        NativeArray<Entity> enemyArray = entityManager.GetAllEntities(Allocator.Temp);
        Debug.Log($"enemies amount {enemyArray.Length}");
        int removeAmount = Mathf.Clamp(enemyArray.Length - _spawnCount, 0, enemyArray.Length);

        for (int i = removeAmount; i > -1; i--)
        {
            entityManager.DestroyEntity(enemyArray[i]);
        }

        enemyArray = entityManager.GetAllEntities(Allocator.Temp);
        _mainUI.SetTotalEnemyValue(enemyArray.Length);
        enemyArray.Dispose();
    }
}