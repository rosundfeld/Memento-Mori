using UnityEngine;

public class Unit : MonoBehaviour
{
	public float health;
	public float maxHealth;
	public float attackDamage;
	public float attackSpeed;
	public float attackRange;
	public float moveSpeed;

	private Unit targetEnemy;
	[SerializeField]  private float attackTimer;

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0) Die();
	}

	public void Attack(Unit target)
	{
		if (targetEnemy == null)
			targetEnemy = target;
	}

	private void Die()
	{
		Destroy(gameObject);
	}
}
