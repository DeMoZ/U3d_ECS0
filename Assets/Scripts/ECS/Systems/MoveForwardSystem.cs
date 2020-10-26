using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
public class MoveForwardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // simple movement for simple objects (bullets)
        Entities.WithNone<BehaviourStatePatrolling, BehaviourStateChasing, BehaviourStateAttacking>().WithAll<MoveForward>().ForEach((
         ref Translation trans,
         ref Rotation rot,
         ref MoveForward moveForward
         ) =>
     {
         trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
     });

        // Behaviour states movement
        Entities.WithAny<BehaviourStatePatrolling, BehaviourStateChasing>().ForEach((
             ref Translation trans,
             ref Rotation rot,
             ref MoveForward moveForward
             ) =>
         {
             trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
         });

    }
}
