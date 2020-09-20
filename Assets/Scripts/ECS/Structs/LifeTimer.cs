using Unity.Entities;

[GenerateAuthoringComponent]
public struct LifeTimer : IComponentData
{
    public float Time;
}