using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance { get; private set; }

    [Header("Faith Resource")]
    [SerializeField] private int maxFaith = 100;
    [SerializeField] private int currentFaith = 100;
    [SerializeField] private float faithRegenPerSecond = 2f;

    [Header("Skills")]
    [SerializeField] private List<SkillDataSO> availableSkills = new List<SkillDataSO>();

    [Header("References")]
    [SerializeField] private GridFormationSystem gridSystem;

    // Cooldowns ativos (skillName -> tempo restante)
    private Dictionary<string, float> activeCooldowns = new Dictionary<string, float>();
    
    // Skill sendo preparada para usar (aguardando seleção de célula)
    private SkillDataSO pendingSkill = null;

    // Eventos para UI
    public event Action<int, int> OnFaithChanged; // currentFaith, maxFaith
    public event Action<SkillDataSO, float> OnCooldownStarted; // skill, duration
    public event Action<SkillDataSO> OnCooldownEnded; // skill
    public event Action<SkillDataSO> OnSkillCastStarted; // skill sendo preparada
    public event Action OnSkillCastCancelled; // cast cancelado

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (gridSystem == null)
        {
            gridSystem = FindFirstObjectByType<GridFormationSystem>();
        }
    }

    private void Start()
    {
        currentFaith = maxFaith;
        OnFaithChanged?.Invoke(currentFaith, maxFaith);
    }

    private void Update()
    {
        // Regenera Fé ao longo do tempo
        if (currentFaith < maxFaith)
        {
            AddFaith(Mathf.CeilToInt(faithRegenPerSecond * Time.deltaTime));
        }

        // Atualiza cooldowns
        UpdateCooldowns();

        // Se há skill pendente, aguarda clique no grid
        if (pendingSkill != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryExecutePendingSkill();
        }

        // Cancela skill pendente com botão direito ou ESC
        if (pendingSkill != null && 
            (UnityEngine.InputSystem.Mouse.current.rightButton.wasPressedThisFrame || 
             UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame))
        {
            CancelPendingSkill();
        }
    }

    #region Faith Management

    public int CurrentFaith => currentFaith;
    public int MaxFaith => maxFaith;

    public void AddFaith(int amount)
    {
        currentFaith = Mathf.Clamp(currentFaith + amount, 0, maxFaith);
        OnFaithChanged?.Invoke(currentFaith, maxFaith);
    }

    public bool SpendFaith(int amount)
    {
        if (currentFaith >= amount)
        {
            currentFaith -= amount;
            OnFaithChanged?.Invoke(currentFaith, maxFaith);
            return true;
        }
        return false;
    }

    #endregion

    #region Cooldown Management

    private void UpdateCooldowns()
    {
        List<string> expiredCooldowns = new List<string>();

        foreach (var kvp in activeCooldowns)
        {
            activeCooldowns[kvp.Key] -= Time.deltaTime;

            if (activeCooldowns[kvp.Key] <= 0)
            {
                expiredCooldowns.Add(kvp.Key);
            }
        }

        // Remove cooldowns expirados e notifica UI
        foreach (string skillName in expiredCooldowns)
        {
            activeCooldowns.Remove(skillName);
            SkillDataSO skill = availableSkills.Find(s => s.skillName == skillName);
            if (skill != null)
            {
                OnCooldownEnded?.Invoke(skill);
            }
        }
    }

    public bool IsOnCooldown(SkillDataSO skill)
    {
        return activeCooldowns.ContainsKey(skill.skillName);
    }

    public float GetCooldownRemaining(SkillDataSO skill)
    {
        return activeCooldowns.TryGetValue(skill.skillName, out float remaining) ? remaining : 0f;
    }

    private void StartCooldown(SkillDataSO skill)
    {
        activeCooldowns[skill.skillName] = skill.cooldown;
        OnCooldownStarted?.Invoke(skill, skill.cooldown);
    }

    #endregion

    #region Skill Execution

    /// <summary>
    /// Tenta usar uma skill. Se precisar de targeting, aguarda seleção de célula.
    /// </summary>
    public bool TryUseSkill(SkillDataSO skill)
    {
        // Validações
        if (skill == null)
        {
            Debug.LogWarning("Skill é null!");
            return false;
        }

        // Verifica se combate está ativo (se necessário)
        if (skill.requiresCombat && CombatManager.Instance != null && !CombatManager.Instance.IsCombatActive)
        {
            Debug.Log($"Skill '{skill.skillName}' requer combate ativo!");
            return false;
        }

        // Verifica cooldown
        if (IsOnCooldown(skill))
        {
            Debug.Log($"Skill '{skill.skillName}' está em cooldown! Restam {GetCooldownRemaining(skill):F1}s");
            return false;
        }

        // Verifica custo de Fé
        if (!skill.CanAfford(currentFaith))
        {
            Debug.Log($"Fé insuficiente para '{skill.skillName}'! Necessário: {skill.faithCost}, Atual: {currentFaith}");
            return false;
        }

        // Se skill tem targeting manual, aguarda seleção
        if (skill.targetRange > 0)
        {
            pendingSkill = skill;
            OnSkillCastStarted?.Invoke(skill);
            Debug.Log($"Selecione uma célula-alvo para '{skill.skillName}' (raio: {skill.targetRange})");
            return true;
        }

        // Executa skill sem targeting (auto-target ou global)
        ExecuteSkill(skill, Vector2Int.zero);
        return true;
    }

    private void TryExecutePendingSkill()
    {
        if (pendingSkill == null || gridSystem == null) return;

        Vector3 mouseWorld = CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition();
        Vector2Int targetCell = gridSystem.GetCellFromWorldPosition(mouseWorld);

        // Valida range (se implementar depois)
        // Por ora, aceita qualquer célula no grid

        ExecuteSkill(pendingSkill, targetCell);
        pendingSkill = null;
    }

    private void CancelPendingSkill()
    {
        if (pendingSkill != null)
        {
            Debug.Log($"Uso de '{pendingSkill.skillName}' cancelado.");
            pendingSkill = null;
            OnSkillCastCancelled?.Invoke();
        }
    }

    private void ExecuteSkill(SkillDataSO skill, Vector2Int targetCell)
    {
        // Gasta Fé
        if (!SpendFaith(skill.faithCost))
        {
            Debug.LogError($"Falha ao gastar Fé para '{skill.skillName}'!");
            return;
        }

        // Executa efeito
        skill.Execute(targetCell, gridSystem, this);

        // Inicia cooldown
        StartCooldown(skill);

        Debug.Log($"Skill '{skill.skillName}' usada em {targetCell}! Fé restante: {currentFaith}");
    }

    #endregion

    #region Public Utilities

    public List<SkillDataSO> GetAvailableSkills() => availableSkills;

    public bool CanUseSkill(SkillDataSO skill)
    {
        if (skill == null) return false;
        if (IsOnCooldown(skill)) return false;
        if (!skill.CanAfford(currentFaith)) return false;
        if (skill.requiresCombat && CombatManager.Instance != null && !CombatManager.Instance.IsCombatActive) return false;
        return true;
    }

    #endregion
}
