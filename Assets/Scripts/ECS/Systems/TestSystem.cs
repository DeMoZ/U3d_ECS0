using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TestSystem
    //: SystemBase
{
    //private EntityQuery m_Group;

    //protected override void OnCreate()
    //{
    //    m_Group = GetEntityQuery(
    //    ComponentType.ReadOnly<Destructable>(),
    //    ComponentType.ReadOnly<Translation>());
    //}

    //protected override void OnUpdate()
    //{
    //    ////  var subjectType = GetArchetypeChunkComponentType<Destructable>();
    //    //// var translationType = GetArchetypeChunkComponentType<Translation>();
    //    //var chunks = m_Group.CreateArchetypeChunkArray(Allocator.TempJob);

    //    //Entities
    //    //    //.WithReadOnly(subjectType)
    //    //    //.WithReadOnly(translationType)
    //    //    //.WithDeallocateOnJobCompletion(chunks)
    //    //    .ForEach((ref Destructor observer, in Translation translation) =>
    //    //    {

    //    //        //observer.Value = 0;

    //    //        for (int c = 0; c < chunks.Length; c++)
    //    //        {
    //    //            var chunk = chunks[c];

    //    //            //var subjects = chunk.GetNativeArray(subjectType);
    //    //            //var translations = chunk.GetNativeArray(translationType);

    //    //            //for (int i = 0; i < chunk.Count; i++)
    //    //            //{
    //    //            //    if (subjects[i].Active)
    //    //            //    {
    //    //            //        float3 distance = translations[i].Value - translation.Value;
    //    //            //        float R = math.dot(distance, distance);
    //    //            //    }
    //    //            //}
    //    //        }

    //    //    }).ScheduleParallel();
    //}
}