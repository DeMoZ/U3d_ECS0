using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class InCameraViewPortSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<ViewPortAttend>().ForEach((ref Translation tran) =>
            {
                // собрать все положения в массив и передать. Получатель расчитает точки относительно камеры            
                CameraBeh.instance.CheckForViewPort(tran.Value);
            });
    }
}
