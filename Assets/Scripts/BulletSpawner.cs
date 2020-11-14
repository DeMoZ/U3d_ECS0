using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class BulletSpawner : MonoBehaviour
{
    public static BulletSpawner instance;

    [SerializeField] private GameObject _bulletPrefab;
    public Entity BulletEntityPrefab { get; private set; }

    private EntityManager entityManager;

    private void Start()
    {
        instance = this;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        BulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_bulletPrefab, settings);
    }

    public void PlusEntities(float3 pos, quaternion rot)
    {
        Entity bullet = entityManager.Instantiate(BulletEntityPrefab);
        entityManager.SetComponentData(bullet, new Translation { Value = pos });
        entityManager.SetComponentData(bullet, new Rotation { Value = rot });
    }
}