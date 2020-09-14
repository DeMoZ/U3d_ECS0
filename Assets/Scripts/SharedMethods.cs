using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class SharedMethods : MonoBehaviour
{
    public static  float MakeRandom(float2 patrolingTimeRange, string str = "")
    {
        return MakeRandom(patrolingTimeRange.x, patrolingTimeRange.y, str);
    }
    public static float MakeRandom(float x, float y, string str = "")
    {
        //float rnd = UnityEngine.Random.Range(x, y);
        //UnityEngine.Debug.Log($"MakeRandom {rnd}, {str}");
        //return rnd;
        return UnityEngine.Random.Range(x, y);
    }
}
