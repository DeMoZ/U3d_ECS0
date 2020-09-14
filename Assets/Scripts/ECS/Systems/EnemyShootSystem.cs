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
        //Entities.WithAll<TurnTimer>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward, ref TurnTimer turnTimer) =>
        Entities.WithAll<ShootTimer>().ForEach((ref Translation trans, ref Rotation rot,  ref ShootTimer shootTimer) =>
        {
            shootTimer.TimerCounter -= Time.DeltaTime;
            //on timer out change rotationt
            if (shootTimer.TimerCounter <= 0)
            {
                shootTimer.TimerCounter = SharedMethods.MakeRandom(shootTimer.TimeRange, shootTimer.TimeRange + "timer");

                Entity entity = entityManager.Instantiate(BulletSpawner.instance.BulletEntityPrefab);
                entityManager.SetComponentData(entity, new Translation { Value = trans.Value });
                entityManager.SetComponentData(entity, new Rotation { Value = rot.Value });
            }
        });
    }


}
