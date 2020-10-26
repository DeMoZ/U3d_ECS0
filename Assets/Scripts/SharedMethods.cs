using Unity.Mathematics;

public static class SharedMethods
{
    public static float MakeRandom(float2 patrolingTimeRange, string str = "", bool log = false)
    {
        return MakeRandom(patrolingTimeRange.x, patrolingTimeRange.y, str);
    }

    public static float MakeRandom(float x, float y, string str = "", bool log = false)
    {
        if (log)
        {
            float rnd = UnityEngine.Random.Range(x, y);
            return rnd;
        }
        else
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

    public static Random _Random = new Random();

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
        float angle = _Random.NextFloat() * math.PI * 2;
        float distance = math.sqrt(_Random.NextFloat()) * radius;
        float x = distance * math.cos(angle);
        float z = distance * math.sin(angle);
        return new float3(x, 0, z);
    }

    public static quaternion RotateTowardsTarget(float3 myPos, quaternion myRot, float3 targetPoint, float turningSpeed, float deltaTime)
    {
        float3 direction = targetPoint - myPos;
        direction.y = 0;
        quaternion disairedRotation = quaternion.LookRotation(direction, math.up());
        quaternion rot = math.nlerp(myRot, disairedRotation, deltaTime * turningSpeed);
        return rot;
    }
}
