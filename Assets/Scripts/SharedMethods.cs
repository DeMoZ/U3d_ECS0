using UnityEngine;
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
            Debug.Log($"{log} {rnd}");
            return rnd;
        }
        else
            return UnityEngine.Random.Range(x, y);
    }

    public static Quaternion RandomRotation()
    {
        float3 euler = new float3(0, UnityEngine.Random.Range(-180, 180), 0);

        return Quaternion.Euler(euler);
    }

    public static float3 RandomPointOnCircle(object spawnRadius)
    {
        return new float3(UnityEngine.Random.Range(-10, 10),
                          1f,
                          UnityEngine.Random.Range(-10, 10));
    }
}