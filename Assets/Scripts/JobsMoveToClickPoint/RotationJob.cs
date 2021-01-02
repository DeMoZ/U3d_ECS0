using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct RotationJob : IJobParallelForTransform
    {
        private const float RotationSpeed = 0.1f;

        public NativeArray<Quaternion> Rotations;

        public Vector3 TargetPosition;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 direction = LookDirection(transform);
            
            Quaternion rotationVector = Quaternion.LookRotation(direction);
            Quaternion rotation = Quaternion.Lerp(Rotations[index], rotationVector, RotationSpeed * DeltaTime);

            Rotations[index] = rotation;
            transform.rotation = Rotations[index];
        }

        private Vector3 LookDirection(TransformAccess transform)
        {
            Vector3 targetPosition = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);
            return targetPosition - transform.position;
        }
    }
}