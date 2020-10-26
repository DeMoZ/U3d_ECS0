using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BehaviourStatePatrolling : IComponentData
{
    public float3? TargetPoint;
}
