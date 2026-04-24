using UnityEngine;

[CreateAssetMenu(fileName = "UnitySpawnData", menuName = "Scriptable Objects/UnitySpawnData")]
public class UnitySpawnData : ScriptableObject
{
	public GameObject UnitPrefab;

	public float health;
	public float mana;
	public float dmg;

	[Tooltip("Range em unidades do mundo (metros). Usado para setar um Collider ou para Overlap checks.")]
	[Range(0f, 50f)]
	public float range;
}
