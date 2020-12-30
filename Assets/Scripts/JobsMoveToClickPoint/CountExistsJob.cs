using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace JobsMoveToClickPoint
{
    [BurstCompile]
    public struct CountExistsJob : IJob
    {
        [ReadOnly] public NativeArray<int> Exists;
        public NativeArray<int> NumberOfBots;

        public void Execute()
        {
            for (int index = 0; index < Exists.Length; index++)
            {
                if (Exists[index] != 0)
                {
                    var count = NumberOfBots[0] + 1;
                    NumberOfBots[0] = count;
                }
            }
        }
    }
}