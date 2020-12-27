using Unity.Collections;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    public interface IBotFactory
    {
        Transform[] GenerateBots(int numberOfBots, ref NativeArray<Quaternion> rotations, ref NativeArray<Vector3> positions);
    }
}