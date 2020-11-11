using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class ChaseSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem barrier;

    protected override void OnCreate()
    {
        barrier = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var commandBuffer = barrier.CreateCommandBuffer();

        Entities.WithAll<BehaviourStateChasing>().ForEach((
            Entity entity, ref Translation translation, ref Rotation rotation, in BehaviourStateChasing behaviourStateChasing,
            in Turning turning, in Speed speed, in AttackDistance attackDistance
            ) =>
        {
            if (!HasComponent<LocalToWorld>(behaviourStateChasing.Target))
            {
                commandBuffer.RemoveComponent<BehaviourStateChasing>(entity);
                commandBuffer.AddComponent<BehaviourStatePatrolling>(entity);
            }
            else
            {
                // checks for attack
                var myPos = translation.Value;
                var targetTransform = GetComponent<LocalToWorld>(behaviourStateChasing.Target);
                var targetPos = targetTransform.Position;
                var myAttackDistance = attackDistance.Value;
                var turningSpeed = turning.TurningSpeed;
                var myRot = rotation.Value;
                // rotation
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, targetPos, turningSpeed, deltaTime);

                // movement
                translation.Value += speed.PatrollSpeed * deltaTime * math.forward(rotation.Value);

                // checks for attack
                if (SharedMethods.CanAttackTarget(myPos, targetPos, myAttackDistance))
                {
                    commandBuffer.RemoveComponent<BehaviourStateChasing>(entity);
                    commandBuffer.AddComponent(entity, new BehaviourStateAttacking { Target = behaviourStateChasing.Target });
                }
            }
        }).Run();
    }
}
