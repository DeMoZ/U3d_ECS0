using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Turn : IComponentData
{
    public float RotationSpeed;

    /// <summary>
    /// disaired rotation. Target rotation for next tarn.
    /// </summary>
    public quaternion TurgetRotation { get; set; }
    public float2 TimeRange;

    /// <summary>
    /// Countdown from _PatrolingTimeRange random
    /// </summary>
    public float TimerCounter { get; set; }
}
