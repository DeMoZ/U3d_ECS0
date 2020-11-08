using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class ChaseSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<BehaviourStateChasing>().ForEach((
            Entity entity, ref Translation translation, ref Rotation rotation, ref BehaviourStateChasing behaviourStateChasing,
            ref Turning turning, ref Speed speed, ref AttackDistance attackDistance
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;

            if (EntityManager.HasComponent(behaviourStateChasing.Target, typeof(Translation)))
            {
                float3 turgetPoint = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(behaviourStateChasing.Target).Value;

                // rotation
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, turgetPoint, turningSpeed, Time.DeltaTime);

                // movement
                translation.Value += speed.PatrollSpeed * deltaTime * math.forward(rotation.Value);

                // checks for attack
                float3 targetPos = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(behaviourStateChasing.Target).Value;
                float myAttackDistance = attackDistance.Value;

                if (SharedMethods.CanAttackTarget(myPos, targetPos, myAttackDistance))
                {
                    EntityManager.RemoveComponent(entity, typeof(BehaviourStateChasing));
                    EntityManager.AddComponentData(entity, new BehaviourStateAttacking { Target = behaviourStateChasing.Target });
                }
            }
            else
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStateChasing));
                EntityManager.AddComponent(entity, typeof(BehaviourStatePatrolling));
            }
        });
    }
}
