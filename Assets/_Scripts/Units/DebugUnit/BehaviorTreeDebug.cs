using UnityEngine;

public class BehaviorTreeDebugger : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Unit[] units = FindObjectsByType<Unit>(FindObjectsSortMode.None);

		foreach (var unit in units)
		{
			// Desenha range de visão
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(unit.transform.position, 20f);

			// Desenha range de ataque
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(unit.transform.position, unit.attackRange);

			// Linha até alvo
			//if (unit.GetTarget() != null)
			//{
			//	Gizmos.color = Color.green;
			//	Gizmos.DrawLine(unit.transform.position, unit.GetTarget().transform.position);
			//}
		}
	}
}
