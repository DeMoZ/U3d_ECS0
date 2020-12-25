using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsScripts
{
    public struct AccelerationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Positions;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [WriteOnly] public NativeArray<Vector3> Accelerations;

        /// <summary>
        /// distance to follow target
        /// </summary>
        public float DestinationThreshold;

        private int Count =>
            Positions.Length - 1;

        public void Execute(int index)
        {
            Vector3 averageSpread = Vector3.zero,
                averageVelocity = Vector3.zero,
                averagePosition = Vector3.zero;

            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;

                Vector3 posDifference = Positions[index] - Positions[i];

                if (posDifference.magnitude > DestinationThreshold)
                    continue;

                averageSpread += posDifference.normalized;
                averageVelocity += Velocities[i];
                averagePosition += Positions[i];
            }

            Accelerations[index] =
                averageSpread / Count +
                averageVelocity / Count +
                averagePosition / Count - Positions[index];
        }
    }
}