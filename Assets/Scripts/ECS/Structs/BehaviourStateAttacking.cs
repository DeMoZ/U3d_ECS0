using Unity.Entities;

[GenerateAuthoringComponent]
public struct BehaviourStateAttacking : IComponentData
{
    public Entity Target;
}
