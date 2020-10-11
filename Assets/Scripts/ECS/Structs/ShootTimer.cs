using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ShootTimer : IComponentData
{
    public float2 TimeRange;
    public float TimerCounter;//{ get; set; }
}
