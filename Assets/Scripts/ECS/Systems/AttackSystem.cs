
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class AttackSystem : SystemBase
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
        var bulletPrefab = BulletSpawner.instance.BulletEntityPrefab;

        Entities
        //.WithoutBurst() // if you need to access mainthread - uncomment this
        .WithAll<BehaviourStateAttacking>()
        .ForEach((Entity entity, ref Rotation rotation, ref ShootTimer shootTimer,
                  in AttackDistance attackDistance, in Turning turning, in BehaviourStateAttacking behaviourStateAttacking
            ) =>
        {
            if (!HasComponent<LocalToWorld>(behaviourStateAttacking.AttackTarget))
            {
                commandBuffer.RemoveComponent<BehaviourStateAttacking>(entity);
                commandBuffer.AddComponent<BehaviourStatePatrolling>(entity);
            }
            else
            {
                // checks for attack
                LocalToWorld myTransform = GetComponent<LocalToWorld>(entity);
                float3 myPos = myTransform.Position;
                LocalToWorld targetTransform = GetComponent<LocalToWorld>(behaviourStateAttacking.AttackTarget);
                float3 targetPos = targetTransform.Position;
                float myAttackDistance = attackDistance.Value;
                float turningSpeed = turning.TurningSpeed;
                quaternion myRot = rotation.Value;

                if (!SharedMethods.CanAttackTarget(myPos, targetPos, myAttackDistance)) // this should be a static or public method in the system
                {
                    commandBuffer.RemoveComponent<BehaviourStateAttacking>(entity);
                    commandBuffer.AddComponent(entity, new BehaviourStateChasing { ChaseTarget = behaviourStateAttacking.AttackTarget });
                }

                // rotation 
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, targetPos, turningSpeed, deltaTime);

                // shooting
                shootTimer.TimerCounter -= deltaTime;
                shootTimer.TimerCounter = shootTimer.TimerCounter < 0 ? 0 : shootTimer.TimerCounter;

                if (shootTimer.TimerCounter <= 0)
                {
                    shootTimer.TimerCounter = SharedMethods.MakeRandom(shootTimer.TimeRange);

                    Entity bulletEntity = commandBuffer.Instantiate(bulletPrefab);

                    float3 bulletPos = myPos + math.mul(rotation.Value, new float3(0f, 0f, 1.5f));
                    commandBuffer.SetComponent(bulletEntity, new Translation { Value = bulletPos });
                    commandBuffer.SetComponent(bulletEntity, new Rotation { Value = rotation.Value });
                }
            }
        }).Run();
    }
}


/*
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
public class AttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BehaviourStateAttacking>().ForEach((
            Entity entity, ref Translation translation, ref Rotation rotation, ref BehaviourStateAttacking behaviourStateAttacking,
            ref Turning turning, ref ShootTimer shootTimer, ref AttackDistance attackDistance
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;
            Entity targetEntity = behaviourStateAttacking.AttackTarget;
            float myAttackDistance = attackDistance.Value;


            if (EntityManager.HasComponent(behaviourStateAttacking.AttackTarget, typeof(Translation)))
            {
                // checks for attack
                float3 targetPos = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(behaviourStateAttacking.AttackTarget).Value;

                if (!SharedMethods.CanAttackTarget(myPos, targetPos, myAttackDistance))
                {
                    EntityManager.RemoveComponent(entity, typeof(BehaviourStateAttacking));
                    EntityManager.AddComponentData(entity, new BehaviourStateChasing { ChaseTarget = targetEntity });
                }

                // rotation
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, targetPos, turningSpeed, Time.DeltaTime);

                // shooting 
                shootTimer.TimerCounter -= Time.DeltaTime;
                shootTimer.TimerCounter = shootTimer.TimerCounter < 0 ? 0 : shootTimer.TimerCounter;

                if (shootTimer.TimerCounter <= 0)
                {
                    shootTimer.TimerCounter = SharedMethods.MakeRandom(shootTimer.TimeRange, shootTimer.TimeRange + "timer");

                    Entity bulletEntity = EntityManager.Instantiate(BulletSpawner.instance.BulletEntityPrefab);

                    float3 bulletPos = translation.Value + math.mul(rotation.Value, new float3(0f, 0f, 1.5f));
                    EntityManager.SetComponentData(bulletEntity, new Translation { Value = bulletPos });
                    EntityManager.SetComponentData(bulletEntity, new Rotation { Value = rotation.Value });
                }
            }
            else
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStateAttacking));
                EntityManager.AddComponent(entity, typeof(BehaviourStatePatrolling));
            }
        });
    }
}
*/