using Unity.Entities;

public class LifeTimerSystem : ComponentSystem
{
    private EntityManager entityManager;

    protected override void OnStartRunning()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<LifeTimer>().ForEach((Entity bullet, ref LifeTimer lifeTimer) =>
        {
            lifeTimer.Time -= Time.DeltaTime;

            if (lifeTimer.Time <= 0)
                entityManager.DestroyEntity(bullet);
        });
    }
}
