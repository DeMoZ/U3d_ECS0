using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct InitializeExistsJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<int> Exists;
        
        public void Execute(int index)
        {
            Exists[index] = 1;
        }
    }
}