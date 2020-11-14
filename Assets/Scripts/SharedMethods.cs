using Unity.Mathematics;

public static class SharedMethods
{

    public static float MakeRandom(float2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }

    public static float MakeRandom(float x, float y)
    {
        return UnityEngine.Random.Range(x, y);
    }

    public static quaternion RandomRotation()
    {
        float3 euler = new float3(0, UnityEngine.Random.Range(-180, 180), 0);

        return quaternion.Euler(euler);
    }

    public static float3 RandomPointOnCircle(object spawnRadius)
    {
        return new float3(UnityEngine.Random.Range(-10, 10),
                          1f,
                          UnityEngine.Random.Range(-10, 10));
    }

    public static Random _Random = new Random(1);

    public static float3 RandomPointOnCircle(float3 center, float radius)
    {
        float angle = _Random.NextFloat() * math.PI * 2;
        float distance = math.sqrt(_Random.NextFloat()) * radius;
        float x = center.x + distance * math.cos(angle);
        float z = center.z + distance * math.sin(angle);
        return new float3(x, center.y, z);
    }

    public static float3 RandomPointInCircleRadius(float radius)
    {
        float angle = MakeRandom(0, 360);// _Random.NextFloat() * math.PI * 2;
        float distance = MakeRandom(0, 1) * radius;// math.sqrt(_Random.NextFloat()) * radius;
        float x = distance * math.cos(angle);
        float z = distance * math.sin(angle);
        return new float3(x, 0, z);
    }

    public static quaternion RotateTowardsTarget(float3 myPos, quaternion myRot, float3 targetPoint, float turningSpeed, float deltaTime)
    {
        float3 direction = targetPoint - myPos;
        direction.y = 0;
        quaternion disairedRotation = quaternion.LookRotation(direction, math.up());
        //quaternion rot = disairedRotation;
        quaternion rot = math.nlerp(myRot, disairedRotation, deltaTime * turningSpeed);
        return rot;
    }

    public static bool CanAttackTarget(float3 myPos, float3 targetPos, float attackDistance)
    {
        if (math.distance(myPos, targetPos) <= attackDistance)
            return true;

        return false;
    }

    public static float3 GetRandomPatrollingPoint(float3 myPos, float radius)
    {
        float3 newPoint = float3.zero;
        newPoint = myPos;
        newPoint += RandomPointInCircleRadius(radius);

        return newPoint;
    }
}
