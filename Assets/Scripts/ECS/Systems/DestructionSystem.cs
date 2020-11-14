using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DestructionSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _buffer;
    private EntityQuery _group;

    protected override void OnStartRunning()
    {
        _buffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _group = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Destructable>());
    }

    protected override void OnUpdate()
    {
        var commandBuffer = _buffer.CreateCommandBuffer();

        var chunks = _group.CreateArchetypeChunkArray(Allocator.TempJob);
        var destructableType = GetComponentTypeHandle<Destructable>();
        var translationType = GetComponentTypeHandle<Translation>();
        var entitiesType = GetEntityTypeHandle();

        Entities.WithAll<Destructor>().ForEach((Entity vilian, in Translation vilianTrans, in Destructor destructor) =>
        {
            var vilianPos = vilianTrans.Value;

            for (int c = 0; c < chunks.Length; c++)
            {
                var chunk = chunks[c];
                var destructableTypeArray = chunk.GetNativeArray(destructableType);
                var translationTypeArray = chunk.GetNativeArray(translationType);
                var entitiesTypeArray = chunk.GetNativeArray(entitiesType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    if (math.distance(vilianPos, translationTypeArray[i].Value) < 1)
                    {
                        commandBuffer.DestroyEntity(vilian);
                        commandBuffer.DestroyEntity(entitiesTypeArray[i]);
                    }
                }
            }
        }).Run();

        chunks.Dispose();
    }
}
