using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Turning : IComponentData
{
    public float TurningSpeed;
}
