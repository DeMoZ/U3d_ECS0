using System.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct RotationJob : IJobParallelFor
    {
        private const float RotationSpeed = 1f;

        public NativeArray<Quaternion> Rotations;
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<int> Exists;

        public float DeltaTime;
        public Vector3 TargetPosition;

        public void Execute(int index)
        {
            if (Exists[index] == 0) return;

            Vector3 direction = TargetPosition - Positions[index];
            direction.y = 0;
            Quaternion rotation = Quaternion.Lerp(Rotations[index], quaternion.LookRotation(direction, Vector3.up),
                RotationSpeed * DeltaTime);
            Rotations[index] = rotation;
        }
    }
}