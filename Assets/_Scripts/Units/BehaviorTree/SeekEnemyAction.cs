using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SeekEnemy", story: "Agent Seeks enemy at his MoveSpeed", category: "Action", id: "96bd695b356a2bf0b669cd68106282bf")]
public partial class SeekEnemyAction : Action
{
    private Unit unit;
    private UnitTargeting targeting;
    private Animator animator;

    private GameObject Enemy;

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

		Unit best = targeting.FindBestTarget(unit, enemies);

		if (best == null)
            return Status.Failure;

        Enemy = best.gameObject;

        // Se já está no range, não precisa se mover
        if (targeting.IsInRange(unit, best))
            return Status.Success;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (unit == null || Enemy == null)
            return Status.Failure;

        if (!Enemy.TryGetComponent<Unit>(out var target))
            return Status.Failure;

        if (targeting.IsInRange(unit, target))
            return Status.Success;

        // Move em direção ao inimigo
        Vector3 direction = (Enemy.transform.position - unit.transform.position).normalized;
        unit.transform.position += Time.deltaTime * unit.Stats.moveSpeed * direction;
		//this.animator.SetBool("isMoving", true);


		return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

