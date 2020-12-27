using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    public struct VelocityJobNavMesh : IJobParallelFor
    {
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Positions;
        
        public float DeltaTime;

        public Vector3 TargetPosition;
        
        public void Execute(int index)
        {
         //   var desiredVelocity = TargetPosition - Positions[index];
         var desiredVelocity = TargetPosition - Positions[index];
            
            Velocities[index] = Vector3.Lerp(Velocities[index], desiredVelocity, DeltaTime);
        }
    }
}