using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct VelocityJob : IJobParallelFor
    {
        private const float _acceleration = .4f;
        private const float _braking = 1f;
        
        public NativeArray<Vector3> Velocities;
        [ReadOnly] public NativeArray<Vector3> Positions;

        public float DeltaTime;
        public Vector3 TargetPosition;

        public void Execute(int index)
        {
            var direction = TargetPosition - Positions[index];
            direction.y = 0;

            var dot = Vector3.Dot(direction, Velocities[index]);

            var multiplier = dot < 0 ? _braking : _acceleration;
             
            Velocities[index] = Vector3.Lerp(Velocities[index], direction, multiplier * DeltaTime);
        }
    }
}