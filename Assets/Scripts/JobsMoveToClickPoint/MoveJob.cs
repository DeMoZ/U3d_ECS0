using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        [WriteOnly] public NativeArray<Vector3> Positions;
        
        [ReadOnly] public NativeArray<Quaternion> Rotations;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [ReadOnly] public NativeArray<int> Exists;

        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            if (Exists[index] == 0) return;

            transform.position += Velocities[index] * DeltaTime;
            transform.rotation = Rotations[index];
            Positions[index] = transform.position;
        }
    }
}