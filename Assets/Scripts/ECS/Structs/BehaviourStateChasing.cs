using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BehaviourStateChasing : IComponentData
{
    //public float3? ChaseTarget;
    public Entity ChaseTarget;
}
