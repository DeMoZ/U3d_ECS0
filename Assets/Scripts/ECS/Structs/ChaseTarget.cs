using Unity.Entities;

[GenerateAuthoringComponent]
public struct ChaseTarget : IComponentData
{
    public Entity Turget;
}
