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
}
