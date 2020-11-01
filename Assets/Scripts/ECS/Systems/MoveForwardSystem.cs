using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
public class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<MoveForward>().ForEach((
         ref Translation trans,
         in Rotation rot,
         in MoveForward moveForward
         ) =>
     {
         trans.Value += moveForward.speed * deltaTime * math.forward(rot.Value);
     }).Run();

        //// Behaviour states movement
        //Entities.WithAny<BehaviourStatePatrolling, BehaviourStateChasing>().ForEach((
        //     ref Translation trans,
        //     in Rotation rot,
        //     in MoveForward moveForward
        //     ) =>
        // {
        //     trans.Value += moveForward.speed * deltaTime * math.forward(rot.Value);
        // }).Run();
    }
}
