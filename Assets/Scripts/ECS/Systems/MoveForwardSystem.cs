using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
public class MoveForwardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // simple movement for simple objects (bullets)
        Entities.WithNone<BehaviourState>().WithAll<MoveForward>().ForEach((
            ref Translation trans,
            ref Rotation rot,
            ref MoveForward moveForward
            ) =>
        {
            trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
        });

        // Behaviour states movement
        Entities.WithAll<BehaviourState>().ForEach((
            ref Translation trans,
            ref Rotation rot,
            ref MoveForward moveForward,
            ref BehaviourState behaviourState
            ) =>
        {
            if (behaviourState.Value != ProjectEnums.BehaviourState.Attack)
                trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
        });

    }
}
