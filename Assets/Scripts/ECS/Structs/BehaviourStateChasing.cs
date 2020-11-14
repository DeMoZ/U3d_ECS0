using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BehaviourStateChasing : IComponentData
{
    public Entity Target;
}
