using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class PatrollingSystem : SystemBase
{
    private readonly float _patrolingRadius = 10f;
    private readonly float _targetReached = 0.2f; // target reached at this distance
    private Random _random = new Random(1);
    private float _noticeTargetDistance = 10f;

    EndSimulationEntityCommandBufferSystem barrier;

    private EntityQuery m_Group;

    protected override void OnCreate()
    {
        barrier = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_Group = GetEntityQuery(ComponentType.ReadOnly<TeamTag>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var commandBuffer = barrier.CreateCommandBuffer();
        var patrollinRadius = _patrolingRadius;
        var targetReached = _targetReached;
        var noticeTargetDistance = _noticeTargetDistance;

        var chunks = m_Group.CreateArchetypeChunkArray(Allocator.TempJob);
        var teamTagType = GetComponentTypeHandle<TeamTag>();
        var translationType = GetComponentTypeHandle<Translation>();
        var entitiesType = GetEntityTypeHandle();

        Entities.WithAll<BehaviourStatePatrolling>().ForEach((
            Entity entity, ref Translation translation, ref Rotation rotation, ref BehaviourStatePatrolling patrolling,
            in Turning turning, in Speed speed, in TeamTag teamTag
            ) =>
        {
            var myPos = translation.Value;
            var myRot = rotation.Value;
            var turningSpeed = turning.TurningSpeed;

            if (!patrolling.TargetPoint.HasValue)
                patrolling.TargetPoint = SharedMethods.GetRandomPatrollingPoint(myPos, patrollinRadius);

            var targetPoint = (float3)patrolling.TargetPoint;

            // rotation
            rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, targetPoint, turningSpeed, deltaTime);

            // movement
            translation.Value += speed.PatrollSpeed * deltaTime * math.forward(rotation.Value);

            if (math.distance(translation.Value, (float3)patrolling.TargetPoint) <= targetReached)
                patrolling.TargetPoint = SharedMethods.GetRandomPatrollingPoint(myPos, patrollinRadius);

            var myTeam = teamTag.Value;
            var closestTarget = Entity.Null;
            var closestTargetPos = float3.zero;

            for (int c = 0; c < chunks.Length; c++)
            {
                var chunk = chunks[c];
                var teamTagTypeArray = chunk.GetNativeArray(teamTagType);
                var translationTypeArray = chunk.GetNativeArray(translationType);
                var entitiesTypeArray = chunk.GetNativeArray(entitiesType);
                for (int i = 0; i < chunk.Count; i++)
                {
                    if (teamTagTypeArray[i].Value != teamTag.Value)
                    {
                        if (closestTarget == Entity.Null
                        && math.distance(myPos, translationTypeArray[i].Value) < noticeTargetDistance
                        )
                        {
                            closestTarget = entitiesTypeArray[i];//need to get the Entity;
                            closestTargetPos = translationTypeArray[i].Value;
                        }
                    }
                }
            }

            // if target found then chasing
            if (closestTarget != Entity.Null)
            {
                commandBuffer.RemoveComponent<BehaviourStatePatrolling>(entity);
                commandBuffer.AddComponent(entity, new BehaviourStateChasing { Target = closestTarget });
            }
        }).Run();

        chunks.Dispose();
    }
}