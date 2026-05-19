using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Display de Fé (recurso de skills) na UI
/// </summary>
public class FaithDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI faithText;
    [SerializeField] private Image faithBar;
    [SerializeField] private TextMeshProUGUI regenRateText;

    [Header("Format")]
    [SerializeField] private string faithFormat = "{0} / {1} Fé";
    [SerializeField] private bool showRegenRate = true;

    private SkillsManager skillsManager;

    private void Start()
    {
        skillsManager = SkillsManager.Instance;

        if (skillsManager != null)
        {
            skillsManager.OnFaithChanged += OnFaithChanged;
            UpdateDisplay(skillsManager.CurrentFaith, skillsManager.MaxFaith);
        }
    }

    private void OnDestroy()
    {
        if (skillsManager != null)
        {
            skillsManager.OnFaithChanged -= OnFaithChanged;
        }
    }

    private void OnFaithChanged(int current, int max)
    {
        UpdateDisplay(current, max);
    }

    private void UpdateDisplay(int current, int max)
    {
        // Atualiza texto
        if (faithText != null)
        {
            faithText.text = string.Format(faithFormat, current, max);
        }

        // Atualiza barra
        if (faithBar != null)
        {
            faithBar.fillAmount = max > 0 ? (float)current / max : 0f;
        }

        // Atualiza taxa de regeneração (opcional)
        if (showRegenRate && regenRateText != null && skillsManager != null)
        {
            // Você pode expor faithRegenPerSecond como propriedade pública se quiser exibir
            regenRateText.text = "+2 Fé/s"; // Placeholder
        }
    }
}
