using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyShootSystem : ComponentSystem
{
    private EntityManager entityManager;

    protected override void OnStartRunning()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<ShootTimer>().ForEach((ref Translation trans, ref Rotation rot, ref ShootTimer shootTimer) =>
        {
            shootTimer.TimerCounter -= Time.DeltaTime;

            if (shootTimer.TimerCounter <= 0)
            {
                shootTimer.TimerCounter = SharedMethods.MakeRandom(shootTimer.TimeRange, shootTimer.TimeRange + "timer");

                Entity entity = entityManager.Instantiate(BulletSpawner.instance.BulletEntityPrefab);

                float3 newPos = trans.Value + math.mul(rot.Value, new float3(0f, 0f, 1.5f));
                entityManager.SetComponentData(entity, new Translation { Value = newPos });
                entityManager.SetComponentData(entity, new Rotation { Value = rot.Value });
            }
        });
    }

    //float3 euler = new float3(0, UnityEngine.Random.Range(-180, 180), 0);
    //    return Quaternion.Euler(euler);

}
