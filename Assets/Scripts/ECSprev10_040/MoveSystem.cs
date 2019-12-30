using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECSprev10_040
{
    //public class MoveSystem : JobComponentSystem
    //{
    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
    //    {
    //        float deltaTime = Time.DeltaTime;
    //        JobHandle jobHandle= Entities.ForEach((ref Translation translation,in MoveDirection moveDirection) => {
    //            translation.Value.x += moveDirection.Value * deltaTime ;
    //        }).Schedule(inputDeps);

    //        return jobHandle;
    //    }
    //}

    [AlwaysSynchronizeSystem]
    public class MoveSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation translation, in MoveDirection moveDirection) =>
            {
                translation.Value.x += moveDirection.Value * deltaTime;
            }).Run();

            return default;
        }
    }
}
