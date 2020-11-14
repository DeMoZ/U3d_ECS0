using Unity.Entities;
using Unity.Jobs;

public class LifeTimerSystem : SystemBase
{
    EntityCommandBufferSystem m_barrier;

    protected override void OnCreate()
    {
        m_barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var commandBuffer = m_barrier.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTimer lifetimer) =>
        {
            lifetimer.Time -= deltaTime;

            if (lifetimer.Time < 0.0f)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }
        }).ScheduleParallel();

        m_barrier.AddJobHandleForProducer(Dependency);
    }
}
