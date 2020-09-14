using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ShootTimer : IComponentData
{
    public float2 TimeRange { get; set; }

    /// <summary>
    /// Countdown from _PatrolingTimeRange random
    /// </summary>
    public float TimerCounter { get; set; }
}
