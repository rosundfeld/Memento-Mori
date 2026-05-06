using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SeekEnemy", story: "Agent Seeks [enemy] at [Speed]", category: "Action", id: "96bd695b356a2bf0b669cd68106282bf")]
public partial class SeekEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private Unit unit;
    private UnitTargeting targeting;

    protected override Status OnStart()
    {
        unit = GameObject.GetComponent<Unit>();
        targeting = GameObject.GetComponent<UnitTargeting>();

        if (unit == null || targeting == null)
            return Status.Failure;

        // Escolhe o melhor alvo e escreve no Blackboard
        List<Unit> enemies = UnitManager.Instance != null
            ? UnitManager.Instance.GetEnemies(unit)
            : new List<Unit>();

		Debug.Log($"[SeekEnemyAction] Best target found: {(enemies != null ? enemies : "None")}");

		//Unit best = targeting.FindBestTarget(unit, enemies);
		Unit best = targeting.FindBestTarget(unit, enemies);
		Debug.Log($"[SeekEnemyAction] Best target found: {(best != null ? best.name : "None")}");

		if (best == null)
            return Status.Failure;

        Enemy.Value = best.gameObject;

        // Se já está no range, não precisa se mover
        if (targeting.IsInRange(unit, best))
            return Status.Success;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (unit == null || Enemy.Value == null)
            return Status.Failure;

        Unit target = Enemy.Value.GetComponent<Unit>();
        if (target == null)
            return Status.Failure;

        if (targeting.IsInRange(unit, target))
            return Status.Success;

        // Move em direção ao inimigo
        Vector3 direction = (Enemy.Value.transform.position - unit.transform.position).normalized;
        unit.transform.position += direction * Speed.Value * Time.deltaTime;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

