using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class PatrollingSystem : ComponentSystem
{
    private readonly float _patrolingRadius = 10f;
    private readonly float _targetReached = 0.2f; // target reached at this distance
    private Random _random = new Random(1);

    protected override void OnUpdate()
    {
        Entities.WithAll<BehaviourStatePatrolling>().ForEach((
            ref Translation translation,
            ref Rotation rotation,
            ref BehaviourStatePatrolling patrolling,
            ref Turning turning
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;

            if (!patrolling.TargetPoint.HasValue)
                patrolling.TargetPoint = GetPatrollingTargetPoint(myPos);

            float3 targetPoint = (float3)patrolling.TargetPoint;

            // rotation
            rotation.Value =SharedMethods.RotateTowardsTarget(myPos, myRot, targetPoint, turningSpeed,Time.DeltaTime);

            if (math.distance(translation.Value, (float3)patrolling.TargetPoint) <= _targetReached)
                patrolling.TargetPoint = GetPatrollingTargetPoint(myPos);

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
}
