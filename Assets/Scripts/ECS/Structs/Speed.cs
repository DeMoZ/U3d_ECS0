using Unity.Entities;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
    public float PatrollSpeed;
    public float ShaseSpeed;
    public float AttackSpeed;
}
