using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(UnitTargeting))]
public class Unit : MonoBehaviour
{
	[Header("UI")]
	public Image healthBarFill;
	public Image manaBarFill;

	[Header("Stats")]
	[SerializeField]
	private UnitDataSO data;
	[SerializeField]
	private UnitStats stats;
	public float CurrentHealth { get; private set; }
	public UnitStats Stats { get => stats; set => stats = value; }

	[Header("Targeting")]
	private float retargetTimer;
	[SerializeField]
	private float timeToRetarget = 2f;
	private Dictionary<Unit, float> aggroTable = new Dictionary<Unit, float>();

	public Unit CurrentTargetEnemy { get; private set; }

	private UnitTargeting targeting;

	// Acumulador do cooldown de ataque
	private float attackTimer = 0f;

	private void Awake()
	{
		Stats = data.baseStats.Clone();
		CurrentHealth = Stats.maxHealth;
		targeting = GetComponent<UnitTargeting>();

		UnitManager.Instance.Register(this);
		if (targeting == null)
			targeting = GetComponentInChildren<UnitTargeting>(true);

		// Aviso se os componentes de UI não estiverem configurados
		if (healthBarFill == null)
			Debug.LogWarning($"[Unit] healthBarFill não está atribuído em {gameObject.name}. A barra de vida não será atualizada.", this);
		
		if (manaBarFill == null)
			Debug.LogWarning($"[Unit] manaBarFill não está atribuído em {gameObject.name}. A barra de mana não será atualizada.", this);
	}

	private void FixedUpdate()
	{
		retargetTimer += Time.fixedDeltaTime;
		if (retargetTimer >= timeToRetarget)
		{
			RetargetEnemy();
			retargetTimer = 0;
		}

		// Tick do cooldown de ataque (sempre acumula, independente do BT)
		if (attackTimer < Stats.attackSpeed)
			attackTimer += Time.fixedDeltaTime;
	}

	//--------------------------Attack System-----------------

	/// <summary>Retorna true se a unidade pode atacar (cooldown pronto).</summary>
	public bool CanAttack() => attackTimer >= Stats.attackSpeed;

	/// <summary>Executa o ataque no alvo. Use CanAttack() antes de chamar.</summary>
	public void PerformAttack(Unit target)
	{
		if (target == null) return;
		target.TakeDamage(Stats.attackDamage, this);
		attackTimer = 0f;
	}

	/// <summary>Define o alvo atual (usado pelo BT para manter refer�ncia).</summary>
	public void SetTarget(Unit target)
	{
		CurrentTargetEnemy = target;
	}

	//--------------------------Health System-----------------

	public void TakeDamage(float damage)
	{
		Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth - damage}/{Stats.maxHealth}");
		CurrentHealth -= damage;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Stats.maxHealth);
		
		if (healthBarFill != null)
			healthBarFill.fillAmount = CurrentHealth / Stats.maxHealth;
		
		if (CurrentHealth <= 0) Die();
	}

	public void Heal(float healAmount)
	{
		CurrentHealth += healAmount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Stats.maxHealth);
		
		if (healthBarFill != null)
			healthBarFill.fillAmount = CurrentHealth / Stats.maxHealth;
	}

	//------------------Action System-----------------
	public void TakeDamage(float damage, Unit attacker)
	{
		CurrentHealth -= damage;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Stats.maxHealth);
		
		if (healthBarFill != null)
			healthBarFill.fillAmount = CurrentHealth / Stats.maxHealth;
		
		RegisterAggro(attacker, damage);
		if (CurrentHealth <= 0) Die();
	}

	private void Die()
	{
		UnitManager.Instance?.Unregister(this);
		Destroy(gameObject);
	}

	//------------------Aggro System------------------
	public void RegisterAggro(Unit attacker, float amount)
	{
		if (!aggroTable.ContainsKey(attacker))
			aggroTable[attacker] = 0;
		aggroTable[attacker] += amount;
	}

	public bool WasRecentlyAttackedBy(Unit unit) => aggroTable.ContainsKey(unit);

	public float GetAggro(Unit target) =>
		aggroTable.TryGetValue(target, out float val) ? val : 0f;

	//------------------Retarget System------------------
	public void RetargetEnemy()
	{
		if (targeting == null)
		{
			Debug.LogWarning($"[Unit] UnitTargeting nao encontrado em {gameObject.name}.", gameObject);
			return;
		}

		List<Unit> enemies = UnitManager.Instance != null
	 ? UnitManager.Instance.GetEnemies(this)
   : new List<Unit>();

		CurrentTargetEnemy = targeting.FindBestTarget(this, enemies);
	}

	//------------------Get Units System------------------
	public List<Unit> FindEnemiesInScene() =>
		UnitManager.Instance != null ? UnitManager.Instance.GetEnemies(this) : new List<Unit>();

	public List<Unit> FindAlliesInScene() =>
		UnitManager.Instance != null ? UnitManager.Instance.GetAllies(this) : new List<Unit>();
}
