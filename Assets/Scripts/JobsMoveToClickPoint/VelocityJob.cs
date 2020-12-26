using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct VelocityJob : IJobParallelFor
    {
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Positions;
        
        public float DeltaTime;

        public Vector3 TargetPosition;
        
        public void Execute(int index)
        {
            // найти вектор на TargetPosition
            // изменить велосити на вектор до TargetPosition

            var desiredVelocity = TargetPosition - Positions[index];
            
            Velocities[index] = Vector3.Lerp(Velocities[index], desiredVelocity, DeltaTime);
        }
    }
}