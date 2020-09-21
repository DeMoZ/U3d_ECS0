using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemiesPrefabs;
    private List<Entity> _enemyEntitiesPrefabs = new List<Entity>();

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
        foreach (GameObject prefab in _enemiesPrefabs)
        {
            Entity newEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            _enemyEntitiesPrefabs.Add(newEntity);
        }
    }

    public void PlusEntities()
    {
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(_spawnCount, Allocator.Temp);

        int subCount = 0;

        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(_enemyEntitiesPrefabs[subCount]);

            subCount++;

            if (subCount >= _enemyEntitiesPrefabs.Count)
                subCount = 0;

            entityManager.SetComponentData(enemyArray[i], new Translation { Value = SharedMethods.RandomPointOnCircle(_spawnRadius) });
            entityManager.SetComponentData(enemyArray[i], new Rotation { Value = SharedMethods.RandomRotation() });
            entityManager.SetComponentData(enemyArray[i], new TurnTimer { TimeRange = new float2(1, 8) });
            TurnTimer t = entityManager.GetComponentData<TurnTimer>(enemyArray[i]);
            entityManager.SetComponentData(enemyArray[i], new ShootTimer { TimeRange = t.TimeRange, TimerCounter = SharedMethods.MakeRandom(t.TimeRange) });
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