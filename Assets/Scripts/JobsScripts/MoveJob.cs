using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsScripts
{
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions; // позиции всех трансформов
        public NativeArray<Vector3> Velocities; // хранение актуальных векторов движения для каждой из сущностей
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var velocity = Velocities[index]; // переменная для удобства использования
            transform.position += velocity * DeltaTime;
            transform.rotation = Quaternion.LookRotation(velocity);

            // сохраняю позицию в массиве
            Positions[index] = transform.position;
        }
    }

    public struct AccelerationJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [WriteOnly] public NativeArray<Vector3> Acceleratioins;

        public void Execute(int index, TransformAccess transform)
        {
            throw new System.NotImplementedException();
        }
    }
}