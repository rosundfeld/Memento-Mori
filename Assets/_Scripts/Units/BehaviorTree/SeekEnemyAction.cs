using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SeekEnemy", story: "Agent Seeks [enemy]", category: "Action", id: "96bd695b356a2bf0b669cd68106282bf")]
public partial class SeekEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    private Unit unit;
    private GameObject self;

    protected override Status OnStart()
    {
        self = GameObject;

        if (unit == null || Enemy.Value == null)
            return Status.Failure;

        float distance = Vector3.Distance(
            unit.transform.position,
            Enemy.Value.transform.position
        );

        if (distance <= unit.Stats.attackRange)
            return Status.Success;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (unit == null || Enemy.Value == null)
            return Status.Failure;

        float distance = Vector3.Distance(
            unit.transform.position,
            Enemy.Value.transform.position
        );

        return distance <= unit.Stats.attackRange
            ? Status.Success
            : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

