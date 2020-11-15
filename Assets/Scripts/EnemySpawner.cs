using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemiesPrefabs;
    private List<Entity> _enemyEntitiesPrefabs = new List<Entity>();
    [Header("Manualspawn")]
    [Tooltip("Manual spawning Amount ")]
    [SerializeField] private int _manualSpawnAmount = 100;

    [Header("Autospawn")]
    [Tooltip("Amount of creature for autospawning")]
    [SerializeField] private int _autoSpawnAmount = 5;
    [SerializeField] private float _spawnTime = 0.1f;
    private float _spawnTimer;

    [Header("Somehing")]
    [SerializeField] bool _autospawn = true;
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private MainUI _mainUI;

    private EntityManager _entityManager;

    private void Start()
    {
        _spawnTimer = _spawnTime;

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        _mainUI.SetAddAmoutn(_manualSpawnAmount);

        foreach (GameObject prefab in _enemiesPrefabs)
        {
            Entity newEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            _enemyEntitiesPrefabs.Add(newEntity);
        }
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_autospawn && _spawnTimer <= 0)
        {
            _spawnTimer = _spawnTime;

            SpawnEntities(_autoSpawnAmount);
        }
    }
    public void SpawnEntities()
    {
        SpawnEntities(_manualSpawnAmount);
    }
    private void SpawnEntities(int amount)
    {
        int tagCount = 0;

        for (int i = 0; i < amount; i++)
        {
            SpawnEntity(tagCount);

            tagCount++;
            tagCount = tagCount >= _enemyEntitiesPrefabs.Count ? 0 : tagCount;
        }

        CountEntities();
    }

    private void SpawnEntity(int teamTag)
    {
        Entity entity = _entityManager.Instantiate(_enemyEntitiesPrefabs[teamTag]);

        _entityManager.AddComponent<OnTheScene>(entity);
        _entityManager.AddComponent<BehaviourStatePatrolling>(entity);
        _entityManager.SetComponentData(entity, new Translation { Value = SharedMethods.RandomPointOnCircle(_spawnRadius) });
        _entityManager.SetComponentData(entity, new Rotation { Value = SharedMethods.RandomRotation() });
        ShootTimer st = _entityManager.GetComponentData<ShootTimer>(entity);
        st.TimerCounter = SharedMethods.MakeRandom(st.TimeRange);
        _entityManager.SetComponentData(entity, st);
    }


    public void MinusEntities()
    {
        NativeArray<Entity> enemyArray = _entityManager.GetAllEntities(Allocator.Temp);
        Debug.Log($"enemies amount {enemyArray.Length}");
        // int removeAmount = Mathf.Clamp(enemyArray.Length - _manualSpawnAmount, 0, enemyArray.Length);
        int removeAmount = enemyArray.Length - 1;

        for (int i = removeAmount; i > -1; i--)
            if (_entityManager.HasComponent<OnTheScene>(enemyArray[i]))
            {
                _entityManager.DestroyEntity(enemyArray[i]);

            }
        enemyArray = _entityManager.GetAllEntities(Allocator.Temp);
        _mainUI.SetTotalEnemyValue(0);
        enemyArray.Dispose();
    }

    private void CountEntities()
    {
        NativeArray<Entity> entitiesArray = _entityManager.GetAllEntities(Allocator.Temp);
        var count = 0;

        for (int i = 0; i < entitiesArray.Length; i++)
        {
            if (_entityManager.HasComponent<OnTheScene>(entitiesArray[i]))
            {
                count++;
            }
        }
        entitiesArray.Dispose();

        _mainUI.SetTotalEnemyValue(count);
    }
}