using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	public static UnitManager Instance { get; private set; }

	private readonly HashSet<Unit> units = new HashSet<Unit>();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Register(Unit u)
	{
		if (u != null)
			units.Add(u);
	}

	public void Unregister(Unit u)
	{
		if (u != null)
			units.Remove(u);
	}

	// Retorna todas unidades inimigas relativas ao requester (mantive a lógica de tags existente)
	public List<Unit> GetEnemies(Unit requester)
	{
		List<Unit> enemies = new List<Unit>();
		Debug.Log($"GetEnemies: Found {enemies.Count} enemies for requester {requester.name}");
		if (requester == null) return enemies;

		string targetTag = requester.gameObject.CompareTag("Player") ? "Enemy" : "Player";
		foreach (var u in units)
		{
			if (u == null || u == requester) continue;
			if (u.gameObject.CompareTag(targetTag)) enemies.Add(u);
		}
		return enemies;
	}

	public List<Unit> GetAllies(Unit requester)
	{
		List<Unit> allies = new List<Unit>();
		if (requester == null) return allies;

		string targetTag = requester.gameObject.CompareTag("Player") ? "Player" : "Enemy";
		foreach (var u in units)
		{
			if (u == null || u == requester) continue;
			if (u.gameObject.CompareTag(targetTag)) allies.Add(u);
		}
		return allies;
	}

	// Consulta por área (opcional e útil para reduzir candidatos)
	public List<Unit> GetEnemiesInRange(Unit requester, float range)
	{
		List<Unit> enemies = new List<Unit>();
		if (requester == null) return enemies;

		float rsq = range * range;
		string targetTag = requester.gameObject.CompareTag("Player") ? "Enemy" : "Player";

		Vector3 pos = requester.transform.position;
		foreach (var u in units)
		{
			if (u == null || u == requester) continue;
			if (!u.gameObject.CompareTag(targetTag)) continue;
			if ((u.transform.position - pos).sqrMagnitude <= rsq) enemies.Add(u);
		}
		return enemies;
	}
}
