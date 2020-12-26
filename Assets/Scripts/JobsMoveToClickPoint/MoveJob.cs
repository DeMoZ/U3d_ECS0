using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Quaternion> Rotations;
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;

        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += Velocities[index] * DeltaTime;
            transform.rotation = Rotations[index];
            Positions[index] = transform.position;
        }
    }
}