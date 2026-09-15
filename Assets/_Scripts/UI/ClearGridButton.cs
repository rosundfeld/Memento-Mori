using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Botão de UI para limpar todas as unidades colocadas no grid
/// </summary>
[RequireComponent(typeof(Button))]
public class ClearGridButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Settings")]
    [SerializeField] private bool requireConfirmation = true;
    [SerializeField] private string normalText = "Reset";
    [SerializeField] private string confirmText = "Confirm Clear?";
    [SerializeField] private float confirmTimeout = 3f; // segundos para cancelar confirmação

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color confirmColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private GridFormationSystem gridFormationSystem;
    private bool awaitingConfirmation = false;
    private float confirmationTimer = 0f;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(OnButtonClicked);
    }

    private void Start()
    {
        gridFormationSystem = GridFormationSystem.Instance;
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    private void Update()
    {
        // Atualiza estado do botão
        UpdateButtonState();

        // Gerencia timeout de confirmação
        if (awaitingConfirmation)
        {
            confirmationTimer -= Time.deltaTime;
            if (confirmationTimer <= 0f)
            {
                CancelConfirmation();
            }
        }
    }

    private void OnButtonClicked()
    {
        if (gridFormationSystem == null)
        {
            Debug.LogWarning("ClearGridButton: GridFormationSystem não encontrado!");
            return;
        }

        // Se não requer confirmação, limpa imediatamente
        if (!requireConfirmation)
        {
            ClearGrid();
            return;
        }

        // Se já está aguardando confirmação, executa a limpeza
        if (awaitingConfirmation)
        {
            ClearGrid();
            CancelConfirmation();
        }
        else
        {
            // Primeira clique - solicita confirmação
            awaitingConfirmation = true;
            confirmationTimer = confirmTimeout;
            UpdateVisuals();
        }
    }

    private void ClearGrid()
    {
        if (gridFormationSystem != null)
        {
            gridFormationSystem.ClearAllPlacedUnits();
        }
    }

    private void CancelConfirmation()
    {
        awaitingConfirmation = false;
        confirmationTimer = 0f;
        UpdateVisuals();
    }

    private void UpdateButtonState()
    {
        if (button == null) return;

        // Desabilita durante o combate
        bool canClear = CombatManager.Instance == null || !CombatManager.Instance.IsCombatActive;
        button.interactable = canClear;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (button == null) return;

        bool canClear = CombatManager.Instance == null || !CombatManager.Instance.IsCombatActive;

        // Atualiza texto
        if (buttonText != null)
        {
            if (!canClear)
            {
                buttonText.text = "Combat Active";
            }
            else if (awaitingConfirmation)
            {
                buttonText.text = $"{confirmText} ({Mathf.CeilToInt(confirmationTimer)}s)";
            }
            else
            {
                buttonText.text = normalText;
            }
        }

        // Atualiza cor do botão (se tiver Image)
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (!canClear)
            {
                buttonImage.color = disabledColor;
            }
            else if (awaitingConfirmation)
            {
                buttonImage.color = confirmColor;
            }
            else
            {
                buttonImage.color = normalColor;
            }
        }
    }

    #region Public Methods

    /// <summary>
    /// Define se o botão requer confirmação dupla
    /// </summary>
    public void SetRequireConfirmation(bool require)
    {
        requireConfirmation = require;
        if (!require && awaitingConfirmation)
        {
            CancelConfirmation();
        }
    }

    #endregion
}
