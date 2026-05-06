using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UnitTargeting))]
public class Unit : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private UnitData data;
    [SerializeField]
    private UnitStats stats; // runtime
    public float CurrentHealth { get; private set; }
    public UnitStats Stats { get => stats; set => stats = value; }

    [Header("Targeting")]
    private float retargetTimer;
    [SerializeField]
    private float timeToRetarget = 2f;
    private Dictionary<Unit, float> aggroTable = new Dictionary<Unit, float>();

    private Unit currentTargetEnemy;
    
    
    private UnitTargeting targeting;



    private void Awake()
    {
        Stats = data.baseStats.Clone();
        CurrentHealth = Stats.maxHealth;
        targeting = GetComponent<UnitTargeting>();
        if (targeting == null)
            targeting = GetComponentInChildren<UnitTargeting>(true);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) Die();
    }

    public void Attack(Unit target)
    {
        currentTargetEnemy = target;
    }

    private void FixedUpdate()
    {
        retargetTimer += Time.fixedDeltaTime;
        if (retargetTimer >= timeToRetarget)
        {
            RetargetEnemy();
            retargetTimer = 0;
        }
    }


    private void Update()
    {
        if (currentTargetEnemy == null) return;

        Stats.attackSpeed += Time.deltaTime;

        if (Stats.attackSpeed >= 1f / Stats.attackSpeed)
        {
            currentTargetEnemy.TakeDamage(Stats.attackDamage);
            Stats.attackSpeed = 0;
        }
    }

    //------------------Action System-----------------
    public void TakeDamage(float damage, Unit attacker)
    {
        CurrentHealth -= damage;

        RegisterAggro(attacker, damage);

        if (CurrentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    //------------------Aggro System------------------
    public void RegisterAggro(Unit attacker, float amount)
    {
        if (!aggroTable.ContainsKey(attacker))
            aggroTable[attacker] = 0;

        aggroTable[attacker] += amount;
    }

    public bool WasRecentlyAttackedBy(Unit unit)
    {
        return aggroTable.ContainsKey(unit);
    }

    public float GetAggro(Unit target)
    {
        return aggroTable.TryGetValue(target, out float val) ? val : 0f;
    }

	public void RetargetEnemy()
	{
		if (targeting == null)
		{
			Debug.LogWarning($"[Unit] UnitTargeting nao encontrado em {gameObject.name}. Adicione o componente.", gameObject);
			return;
		}

		// Usa UnitManager para obter candidatos; substitua por GetEnemiesInRange(this, Stats.someSight) se quiser filtrar por distância.
		List<Unit> enemies = UnitManager.Instance != null
			? UnitManager.Instance.GetEnemies(this)
			: new List<Unit>();

		currentTargetEnemy = targeting.FindBestTarget(this, enemies);
	}

	//------------------Get Units System------------------
	// Mantive removidas as versões locais que usavam FindGameObjectsWithTag:
	public List<Unit> FindEnemiesInScene()
	{
		// Apenas fallback para compatibilidade; recomenda-se usar UnitManager
		return UnitManager.Instance != null ? UnitManager.Instance.GetEnemies(this) : new List<Unit>();
	}

	public List<Unit> FindAlliesInScene()
	{
		return UnitManager.Instance != null ? UnitManager.Instance.GetAllies(this) : new List<Unit>();
	}
}
