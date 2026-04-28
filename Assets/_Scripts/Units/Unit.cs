using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	[SerializeField] public UnitData data;
	public UnitStats stats; // runtime

    public float CurrentHealth { get; private set; }
	public List<GameObject> treatList;

	public static Unit Instance;
    private Unit targetEnemy;

    private void Awake()
    {
        stats = data.baseStats.Clone();
        CurrentHealth = stats.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) Die();
    }

    public void Attack(Unit target)
    {
        targetEnemy = target;
    }

    private void Update()
    {
        if (targetEnemy == null) return;

        stats.attackSpeed += Time.deltaTime;

        if (stats.attackSpeed >= 1f / stats.attackSpeed)
        {
            targetEnemy.TakeDamage(stats.attackDamage);
            stats.attackSpeed = 0;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
