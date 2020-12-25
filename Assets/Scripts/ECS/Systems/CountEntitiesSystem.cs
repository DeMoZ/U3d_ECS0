using Unity.Entities;
using UnityEngine;

public class CountEntitiesSystem : ComponentSystem
{
    private MainUI _mainIU;
    private float _time = 0.4f;
    private float _timer;
    protected override void OnStartRunning()
    {
        _mainIU = MainUI.instance;
        _timer = _time;
    }

    protected override void OnUpdate()
    {
        if (_timer < 0)
        {
            _timer += _time;

            int countBullets = 0;
            Entities.ForEach((ref Destructor destructor) =>
            {
                countBullets++;
            });

            int countBots = 0;
            Entities.ForEach((ref OnTheScene onTheScene) =>
            {
                countBots++;
            });

            int countEntities = 0;
            Entities.ForEach((Entity entity) =>
            {
                countEntities++;
            });

            _mainIU?.SetValues(countBots, countBullets, countEntities);
        }
        else
        {
            _timer -= Time.DeltaTime;
        }
    }
}
