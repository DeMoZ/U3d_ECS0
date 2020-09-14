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
            
            entityManager.SetComponentData(enemyArray[i], new ShootTimer { TimeRange = new float2(0.5f, 2) });
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
    /*
    private void Start2()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int i = 0; i < enemyNumber; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10),
                                      Random.Range(-10, 10),
                                      Random.Range(-10, 10));

            float rotY = Random.Range(-180, 180);

            GameObject go = Instantiate(_enemyPrefab, pos, Quaternion.EulerAngles(0, rotY, 0));
        }

    }
    */
    /*
    [SerializeField] private Mesh _enemyMesh;
    [SerializeField] private Material _enemyMaterial;
    [SerializeField] private int enemyNumber = 100; 
    private EntityManager entityManager;


    private void Start1()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // 1 задать архетип сущности - наделяем всеми интересующими свойствами
        EntityArchetype archetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld));


        for (int i = 0; i < enemyNumber; i++)
        {
            // 2 создать сущность выбранного архетипа
            Entity entity = entityManager.CreateEntity(archetype);

            // 3 задать параметры сущности - положение/поворот, внешний вид
            entityManager.AddComponentData(entity, new Translation { Value = new float3(-5f, 0.5f, 5f) });

            entityManager.AddComponentData(entity, new Rotation { Value = quaternion.EulerXYZ(new float3(0f, 45f, 0f)) });

            entityManager.AddSharedComponentData(entity, new RenderMesh
            {
                mesh = _enemyMesh,
                material = _enemyMaterial
            });
        }
    }
    */

    /*
private void Start0()
{
    entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    // 1 задать архетип сущности - наделяем всеми интересующими свойствами
    EntityArchetype archetype = entityManager.CreateArchetype(
        typeof(Translation),
        typeof(Rotation),
        typeof(RenderMesh),
        typeof(RenderBounds),
        typeof(LocalToWorld));

    for (int i = 0; i < 1000; i++)
    {
        // 2 создать сущность выбранного архетипа
        Entity entity = entityManager.CreateEntity(archetype);

        // 3 задать параметры сущности - положение/поворот, внешний вид
        entityManager.AddComponentData(entity, new Translation { Value = new float3(-3f, 0.5f, 5f) });

        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.EulerXYZ(new float3(0f, 45f, 0f)) });

        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = _enemyMesh,
            material = _enemyMaterial
        });
    }
}*/
}