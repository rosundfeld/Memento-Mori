using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Classe base abstrata para efeitos de skills
/// </summary>
public abstract class SkillEffect : ScriptableObject
{
    public abstract void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager);
    public abstract string GetDescription();
}

/// <summary>
/// Efeito de cura em área
/// </summary>
[CreateAssetMenu(fileName = "HealAreaEffect", menuName = "Skills/Effects/Heal Area")]
public class HealAreaEffect : SkillEffect
{
    [Header("Heal Settings")]
    [SerializeField] private float healPercentage = 0.12f; // 12% da vida máxima
    [SerializeField] private float duration = 4f; // duração em segundos
    [SerializeField] private int areaRadius = 1; // raio em células (1 = 3x3)

    public override void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager)
    {
        List<Unit> unitsInArea = GetUnitsInArea(targetCell, areaRadius, grid);
        
        foreach (Unit unit in unitsInArea)
        {
            if (unit != null)
            {
                // Cura instantânea ou ao longo do tempo
                float healAmount = unit.Stats.maxHealth * healPercentage;
                unit.StartCoroutine(ApplyHealOverTime(unit, healAmount, duration));
            }
        }

        Debug.Log($"HealAreaEffect: Curou {unitsInArea.Count} unidades em {targetCell} por {healPercentage * 100}% durante {duration}s");
    }

    private IEnumerator ApplyHealOverTime(Unit unit, float totalHeal, float duration)
    {
        float elapsed = 0f;
        float healPerTick = totalHeal / duration;

        while (elapsed < duration && unit != null)
        {
            unit.Heal(healPerTick * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private List<Unit> GetUnitsInArea(Vector2Int center, int radius, GridFormationSystem grid)
    {
        List<Unit> units = new List<Unit>();
        
        // Busca em área quadrada (radius = 1 -> 3x3)
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int cell = new Vector2Int(center.x + x, center.y + y);
                Unit unit = grid.GetUnitAtCell(cell);
                if (unit != null)
                {
                    units.Add(unit);
                }
            }
        }

        return units;
    }

    public override string GetDescription()
    {
        return $"Cura {healPercentage * 100}% da vida máxima em área ({areaRadius * 2 + 1}x{areaRadius * 2 + 1}) durante {duration}s";
    }
}

/// <summary>
/// Efeito de dano mágico em área
/// </summary>
[CreateAssetMenu(fileName = "DamageAreaEffect", menuName = "Skills/Effects/Damage Area")]
public class DamageAreaEffect : SkillEffect
{
    [Header("Damage Settings")]
    [SerializeField] private float totalDamage = 70f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private int areaRadius = 1; // raio em células

    public override void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager)
    {
        List<Unit> unitsInArea = GetUnitsInArea(targetCell, areaRadius, grid);
        
        foreach (Unit unit in unitsInArea)
        {
            if (unit != null)
            {
                unit.StartCoroutine(ApplyDamageOverTime(unit, totalDamage, duration));
            }
        }

        Debug.Log($"DamageAreaEffect: Aplicou {totalDamage} de dano em {unitsInArea.Count} unidades durante {duration}s");
    }

    private IEnumerator ApplyDamageOverTime(Unit unit, float totalDamage, float duration)
    {
        float elapsed = 0f;
        float damagePerTick = totalDamage / duration;

        while (elapsed < duration && unit != null)
        {
            unit.TakeDamage(damagePerTick * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private List<Unit> GetUnitsInArea(Vector2Int center, int radius, GridFormationSystem grid)
    {
        List<Unit> units = new List<Unit>();
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int cell = new Vector2Int(center.x + x, center.y + y);
                Unit unit = grid.GetUnitAtCell(cell);
                if (unit != null)
                {
                    units.Add(unit);
                }
            }
        }

        return units;
    }

    public override string GetDescription()
    {
        return $"Causa {totalDamage} de dano mágico em área ({areaRadius * 2 + 1}x{areaRadius * 2 + 1}) durante {duration}s";
    }
}

/// <summary>
/// Efeito de buff em alvos específicos
/// </summary>
[CreateAssetMenu(fileName = "BuffEffect", menuName = "Skills/Effects/Buff")]
public class BuffEffect : SkillEffect
{
    [Header("Buff Settings")]
    [SerializeField] private float damageBonus = 0.12f; // +12% dano
    [SerializeField] private float speedBonus = 0.10f; // +10% velocidade de ataque
    [SerializeField] private int targetCount = 4; // número de alvos
    [SerializeField] private float duration = 6f;

    public override void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager)
    {
        // Pega as unidades aliadas mais próximas
        List<Unit> allies = GetClosestAllies(targetCell, targetCount, grid);
        
        foreach (Unit unit in allies)
        {
            if (unit != null)
            {
                unit.StartCoroutine(ApplyBuff(unit, duration));
            }
        }

        Debug.Log($"BuffEffect: Aplicou buff em {allies.Count} unidades por {duration}s");
    }

    private IEnumerator ApplyBuff(Unit unit, float duration)
    {
        // Salva stats originais
        float originalDamage = unit.Stats.attackDamage;
        float originalSpeed = unit.Stats.attackSpeed;

        // Aplica buff
        unit.Stats.attackDamage *= (1 + damageBonus);
        unit.Stats.attackSpeed *= (1 + speedBonus);

        Debug.Log($"Buff aplicado em {unit.name}: +{damageBonus * 100}% dano, +{speedBonus * 100}% velocidade");

        yield return new WaitForSeconds(duration);

        // Remove buff (restaura valores originais)
        if (unit != null)
        {
            unit.Stats.attackDamage = originalDamage;
            unit.Stats.attackSpeed = originalSpeed;
            Debug.Log($"Buff removido de {unit.name}");
        }
    }

    private List<Unit> GetClosestAllies(Vector2Int center, int count, GridFormationSystem grid)
    {
        List<Unit> allAllies = UnitManager.Instance.GetAllies(null); // pega todos aliados
        List<Unit> result = new List<Unit>();

        // Ordena por distância da célula alvo
        allAllies.Sort((a, b) =>
        {
            Vector2Int cellA = grid.GetCellFromWorldPosition(a.transform.position);
            Vector2Int cellB = grid.GetCellFromWorldPosition(b.transform.position);
            float distA = Vector2Int.Distance(cellA, center);
            float distB = Vector2Int.Distance(cellB, center);
            return distA.CompareTo(distB);
        });

        // Pega os N mais próximos
        for (int i = 0; i < Mathf.Min(count, allAllies.Count); i++)
        {
            result.Add(allAllies[i]);
        }

        return result;
    }

    public override string GetDescription()
    {
        return $"+{damageBonus * 100}% dano e +{speedBonus * 100}% velocidade de ataque para {targetCount} unidades por {duration}s";
    }
}
