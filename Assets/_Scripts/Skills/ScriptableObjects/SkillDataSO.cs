using UnityEngine;

/// <summary>
/// ScriptableObject que define uma skill completa
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName = "Nova Skill";
    [TextArea(3, 5)]
    public string description = "Descrição da skill";
    public Sprite icon;

    [Header("Costs & Cooldown")]
    [Tooltip("Custo em Fé para usar a skill")]
    public int faithCost = 20;
    [Tooltip("Cooldown global em segundos")]
    public float cooldown = 14f;

    [Header("Targeting")]
    [Tooltip("Raio de alcance para selecionar célula-alvo (0 = auto-target)")]
    public int targetRange = 5;
    [Tooltip("Requer combate ativo para ser usada?")]
    public bool requiresCombat = true;

    [Header("Effect")]
    [Tooltip("Efeito que será aplicado ao usar a skill")]
    public SkillEffect effect;

    /// <summary>
    /// Valida se a skill pode ser usada com a Fe disponível
    /// </summary>
    public bool CanAfford(int currentFaith)
    {
        return currentFaith >= faithCost;
    }

    /// <summary>
    /// Executa o efeito da skill
    /// </summary>
    public void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager)
    {
        if (effect != null)
        {
            effect.Execute(targetCell, grid, skillManager);
        }
        else
        {
            Debug.LogWarning($"Skill '{skillName}' não possui efeito atribuído!");
        }
    }

    /// <summary>
    /// Retorna descrição completa para UI
    /// </summary>
    public string GetFullDescription()
    {
        string effectDesc = effect != null ? effect.GetDescription() : "Sem efeito";
        return $"<b>{skillName}</b>\n{description}\n\n<color=yellow>Custo: {faithCost} Fé</color>\n<color=cyan>CD: {cooldown}s</color>\n\n{effectDesc}";
    }
}
