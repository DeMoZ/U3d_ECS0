﻿using Unity.Entities;
using Unity.Jobs;

public class LifeTimerSystem : SystemBase
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTimer lifetimer) =>
        {
            lifetimer.Time -= deltaTime;

            if (lifetimer.Time < 0.0f)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }
        }).ScheduleParallel();

        m_Barrier.AddJobHandleForProducer(Dependency);
    }
}
