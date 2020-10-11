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

    private EntityManager entityManager;

    private void Start()
    {
        _spawnTimer = _spawnTime;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

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
    }

    private void SpawnEntity(int teamTag)
    {
        Entity entity = entityManager.Instantiate(_enemyEntitiesPrefabs[teamTag]);

        entityManager.SetComponentData(entity, new Translation { Value = SharedMethods.RandomPointOnCircle(_spawnRadius) });
        entityManager.SetComponentData(entity, new Rotation { Value = SharedMethods.RandomRotation() });
    }


    public void MinusEntities()
    {
        NativeArray<Entity> enemyArray = entityManager.GetAllEntities(Allocator.Temp);
        Debug.Log($"enemies amount {enemyArray.Length}");
        int removeAmount = Mathf.Clamp(enemyArray.Length - _manualSpawnAmount, 0, enemyArray.Length);

        for (int i = removeAmount; i > -1; i--)
        {
            entityManager.DestroyEntity(enemyArray[i]);
        }

        enemyArray = entityManager.GetAllEntities(Allocator.Temp);
        _mainUI.SetTotalEnemyValue(enemyArray.Length);
        enemyArray.Dispose();
    }
}