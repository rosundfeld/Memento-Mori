using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttacksUnitInRange", story: "Attacks Unit in range", category: "Action", id: "39837ae6c41e46c284a376311b595014")]
public partial class AttacksUnitInRangeAction : Action
{
	private Unit unit;
	private UnitTargeting targeting;

	protected override Status OnStart()
	{
		unit = GameObject.GetComponent<Unit>();
		targeting = GameObject.GetComponent<UnitTargeting>();

		if (unit == null || targeting == null)
			return Status.Failure;

		// Se não há alvo válido no range, falha (SeekEnemy deve rodar antes)
		if (unit.CurrentTargetEnemy == null || !targeting.IsInRange(unit, unit.CurrentTargetEnemy))
			return Status.Failure;

		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		if (unit == null)
			return Status.Failure;

		Unit target = unit.CurrentTargetEnemy;

		// Alvo morreu ou saiu do range é volta pro Seek
		if (target == null)
			return Status.Failure;

		if (!targeting.IsInRange(unit, target))
			return Status.Failure;

		// Ataca quando o cooldown estiver pronto
		if (unit.CanAttack())
		{
			unit.PerformAttack(target);
		}

		return Status.Running;
	}

	protected override void OnEnd()
	{
	}
}

