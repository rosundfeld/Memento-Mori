using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente de UI para botão de skill individual
/// </summary>
[RequireComponent(typeof(Button))]
public class SkillButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillDataSO skill;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Button button;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color castingColor = new Color(0f, 1f, 0f, 0.8f);

    private SkillsManager skillsManager;
    private bool isCasting = false;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void Start()
    {
        skillsManager = SkillsManager.Instance;

        if (skillsManager != null)
        {
            // Inscreve em eventos
            skillsManager.OnFaithChanged += OnFaithChanged;
            skillsManager.OnCooldownStarted += OnCooldownStarted;
            skillsManager.OnCooldownEnded += OnCooldownEnded;
            skillsManager.OnSkillCastStarted += OnSkillCastStarted;
            skillsManager.OnSkillCastCancelled += OnSkillCastCancelled;
        }

        UpdateVisuals();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);

        if (skillsManager != null)
        {
            skillsManager.OnFaithChanged -= OnFaithChanged;
            skillsManager.OnCooldownStarted -= OnCooldownStarted;
            skillsManager.OnCooldownEnded -= OnCooldownEnded;
            skillsManager.OnSkillCastStarted -= OnSkillCastStarted;
            skillsManager.OnSkillCastCancelled -= OnSkillCastCancelled;
        }
    }

    private void Update()
    {
        UpdateCooldownDisplay();
    }

    private void OnButtonClicked()
    {
        if (skill == null || skillsManager == null) return;

        skillsManager.TryUseSkill(skill);
    }

    private void UpdateVisuals()
    {
        if (skill == null) return;

        // Atualiza ícone
        if (iconImage != null && skill.icon != null)
        {
            iconImage.sprite = skill.icon;
        }

        // Atualiza texto de custo
        if (costText != null)
        {
            costText.text = $"{skill.faithCost} Fé";
        }

        // Atualiza estado do botão
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (skillsManager == null || skill == null || button == null) return;

        bool canUse = skillsManager.CanUseSkill(skill);
        button.interactable = canUse;

        // Atualiza cor visual
        if (iconImage != null)
        {
            if (isCasting)
            {
                iconImage.color = castingColor;
            }
            else if (canUse)
            {
                iconImage.color = normalColor;
            }
            else
            {
                iconImage.color = disabledColor;
            }
        }
    }

    private void UpdateCooldownDisplay()
    {
        if (skillsManager == null || skill == null) return;

        float cooldownRemaining = skillsManager.GetCooldownRemaining(skill);

        // Atualiza texto de cooldown
        if (cooldownText != null)
        {
            if (cooldownRemaining > 0)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = cooldownRemaining.ToString("F1");
            }
            else
            {
                cooldownText.gameObject.SetActive(false);
            }
        }

        // Atualiza overlay de cooldown (fillAmount)
        if (cooldownOverlay != null)
        {
            if (cooldownRemaining > 0)
            {
                cooldownOverlay.gameObject.SetActive(true);
                cooldownOverlay.fillAmount = cooldownRemaining / skill.cooldown;
            }
            else
            {
                cooldownOverlay.gameObject.SetActive(false);
            }
        }
    }

    #region Event Handlers

    private void OnFaithChanged(int current, int max)
    {
        UpdateButtonState();
    }

    private void OnCooldownStarted(SkillDataSO startedSkill, float duration)
    {
        if (startedSkill == skill)
        {
            UpdateButtonState();
        }
    }

    private void OnCooldownEnded(SkillDataSO endedSkill)
    {
        if (endedSkill == skill)
        {
            UpdateButtonState();
        }
    }

    private void OnSkillCastStarted(SkillDataSO castingSkill)
    {
        isCasting = (castingSkill == skill);
        UpdateButtonState();
    }

    private void OnSkillCastCancelled()
    {
        isCasting = false;
        UpdateButtonState();
    }

    #endregion

    #region Editor Helpers

    /// <summary>
    /// Atribui a skill via código (útil para setup dinâmico)
    /// </summary>
    public void SetSkill(SkillDataSO newSkill)
    {
        skill = newSkill;
        UpdateVisuals();
    }

    #endregion
}
