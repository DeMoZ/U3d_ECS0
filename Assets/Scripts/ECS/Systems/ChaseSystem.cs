using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class ChaseSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BehaviourStateChasing>().ForEach((
            Entity entity,
            ref Translation translation,
            ref Rotation rotation,
            ref BehaviourStateChasing behaviourStateChasing,
            ref Turning turning
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;

            if (EntityManager.HasComponent(behaviourStateChasing.ChaseTarget, typeof(Translation)))
            {
                float3 turgetPoint = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(behaviourStateChasing.ChaseTarget).Value;

                // rotation
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, turgetPoint, turningSpeed, Time.DeltaTime);
            }
        });
    }
}
