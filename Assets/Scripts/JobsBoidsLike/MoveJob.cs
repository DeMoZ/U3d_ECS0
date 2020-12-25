using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsBoidsLike
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions; // позиции всех трансформов
        public NativeArray<Vector3> Velocities; // хранение актуальных векторов движения для каждой из сущностей
        public NativeArray<Vector3> Accelerations;

        public float VelocityLimit;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var velocity = Velocities[index] + Accelerations[index] * DeltaTime; // переменная для удобства использования
            var direction = velocity.normalized;
            velocity = direction * Mathf.Clamp(velocity.magnitude, 1f, VelocityLimit);
            
            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(direction);

            Velocities[index] = velocity; 

            // сохраняю позицию в массиве
            Positions[index] = transform.position;
        }
    }
}