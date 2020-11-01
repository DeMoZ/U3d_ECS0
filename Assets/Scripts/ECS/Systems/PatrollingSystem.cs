using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class PatrollingSystem : ComponentSystem
{
    private readonly float _patrolingRadius = 10f;
    private readonly float _targetReached = 0.2f; // target reached at this distance
    private Random _random = new Random(1);
    private float _noticeTargetDistance = 10f;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<BehaviourStatePatrolling>().ForEach((
           Entity entity, ref Translation translation, ref Rotation rotation, ref BehaviourStatePatrolling patrolling,
            ref Turning turning, ref Speed speed, ref TeamTag teamTag
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;

            if (!patrolling.TargetPoint.HasValue)
                patrolling.TargetPoint = GetPatrollingTargetPoint(myPos);

            float3 targetPoint = (float3)patrolling.TargetPoint;

            // rotation
            rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, targetPoint, turningSpeed, Time.DeltaTime);

            // movement
            translation.Value += speed.PatrollSpeed * deltaTime * math.forward(rotation.Value);

            if (math.distance(translation.Value, (float3)patrolling.TargetPoint) <= _targetReached)
                patrolling.TargetPoint = GetPatrollingTargetPoint(myPos);


            ProjectEnums.TeamTag myTeam = teamTag.Value;
            Entity closestTarget = Entity.Null;
            float3 closestTargetPos = float3.zero;

            FindTarget(ref closestTarget, ref closestTargetPos, myTeam, myPos, _noticeTargetDistance);

            // if target found then chasing
            if (closestTarget != Entity.Null)
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStatePatrolling));
                EntityManager.AddComponentData(entity, new BehaviourStateChasing { ChaseTarget = closestTarget });
            }
        });
    }

    private float3 GetPatrollingTargetPoint(float3 myPos)
    {
        float3 newPoint = float3.zero;
        newPoint = myPos;

        newPoint += RandomPointInCircleRadius(_patrolingRadius);

        return newPoint;
    }

    private float3 RandomPointInCircleRadius(float radius)
    {
        float angle = _random.NextFloat() * math.PI * 2;
        float distance = math.sqrt(_random.NextFloat()) * radius;
        float x = distance * math.cos(angle);
        float z = distance * math.sin(angle);
        return new float3(x, 0, z);
    }

    private void FindTarget(ref Entity target, ref float3 position, ProjectEnums.TeamTag myTeam, float3 myPos, float myNoticeTargetDistance)
    {
        Entity closestTarget = Entity.Null;
        float3 closestTargetPos = float3.zero;

        Entities.WithAll<TeamTag>().ForEach((
            Entity targetEntity,
            ref Translation targetTran,
            ref TeamTag targetTeamTag
            ) =>
        {
            if (myTeam != targetTeamTag.Value)
            {
                if (closestTarget == Entity.Null
                && math.distance(myPos, targetTran.Value) < myNoticeTargetDistance
                )
                {
                    closestTarget = targetEntity;
                    closestTargetPos = targetTran.Value;
                }

                if (math.distance(myPos, targetTran.Value) < math.distance(myPos, closestTargetPos)
                      && math.distance(myPos, targetTran.Value) < myNoticeTargetDistance
                      )
                {
                    closestTarget = targetEntity;
                    closestTargetPos = targetTran.Value;
                }

            }
        });

        target = closestTarget;
        position = closestTargetPos;
    }
}
