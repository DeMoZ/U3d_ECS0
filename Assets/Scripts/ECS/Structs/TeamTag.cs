using Unity.Entities;

[GenerateAuthoringComponent]
public struct TeamTag : IComponentData
{
    public ProjectEnums.TeamTag Value;
}
