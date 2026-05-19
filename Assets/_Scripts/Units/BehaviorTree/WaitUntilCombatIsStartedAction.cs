using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait until combat is started", story: "Wait until combat is started", category: "Action", id: "b8f12623bb67b0b7d1ce8936b4890021")]
public partial class WaitUntilCombatIsStartedAction : Action
{

    protected override Status OnStart()
    {
        // Verifica se o combate está ativo
        if (CombatManager.Instance == null || !CombatManager.Instance.IsCombatActive)
            return Status.Failure; // Não faz nada se não estiver em combate
            
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

