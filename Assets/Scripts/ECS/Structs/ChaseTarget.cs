using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct ChaseTarget : IComponentData
{
    public Entity Target;
    public Translation TargetTran;
}
