using Unity.Entities;

[GenerateAuthoringComponent]
public struct NoticeTagetDistance : IComponentData
{
    /// <summary>
    /// The distance from target to be noticed
    /// </summary>
    public float Value;
}

