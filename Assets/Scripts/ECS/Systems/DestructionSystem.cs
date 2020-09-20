using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DestructionSystem : ComponentSystem
{
    private EntityManager entityManager;

    protected override void OnStartRunning()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<Destructor>().ForEach((Entity vilian, ref Translation vilianTrans, ref Destructor destructor) =>
        {
            float3 vilianPos = vilianTrans.Value;

            Entities.WithAll<Destructable>().ForEach((Entity victim, ref Translation victimTrans, ref Destructable destructable) =>
            {
                if (math.distance(vilianPos, victimTrans.Value) < 1)
                {
                    entityManager.DestroyEntity(vilian);
                    entityManager.DestroyEntity(victim);
                }
            });
        });
    }
}
