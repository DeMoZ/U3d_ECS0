using TMPro;
using Unity.Entities;
using UnityEngine;

public class SystemsDisabler : MonoBehaviour
{
    [SerializeField] bool _behaviourSwitchSystem = false;
    [SerializeField] bool _destructionSystem = false;
    [SerializeField] bool _enemyShootSystem = false;
    [SerializeField] bool _enemyTurnSystem = false;
    [SerializeField] bool _inCameraViewPortSystem = false;
    [SerializeField] bool _lifeTimerSystem = false;
    [SerializeField] bool _moveForwardSystem = false;

    private void Awake()
    {
        ActivateSystems();
    }

    private void Update()
    {
        ActivateSystems();
    }

    private void ActivateSystems()
    {
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<BehaviourSwitchSystem>().Enabled = _behaviourSwitchSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<DestructionSystem>().Enabled = _destructionSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<EnemyShootSystem>().Enabled = _enemyShootSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<EnemyTurnSystem>().Enabled = _enemyTurnSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<InCameraViewPortSystem>().Enabled = _inCameraViewPortSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<LifeTimerSystem>().Enabled = _lifeTimerSystem;
        World.DefaultGameObjectInjectionWorld.GetExistingSystem<MoveForwardSystem>().Enabled = _moveForwardSystem;
    }
}
