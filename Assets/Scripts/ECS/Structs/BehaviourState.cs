using Unity.Entities;

[GenerateAuthoringComponent]
public struct BehaviourState : IComponentData
{
    public ProjectEnums.BehaviourState Value;
}
