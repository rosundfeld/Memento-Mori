using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Botao de UI para selecionar unidade no modo de construcao.
/// </summary>
[RequireComponent(typeof(Button))]
public class BuildUnitButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitDataSO unitData;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Button button;

    [Header("Visual Feedback")]
    [SerializeField] private Image selectionHighlight;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private GridFormationSystem gridFormationSystem;

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

        if (gridFormationSystem != null)
        {
            gridFormationSystem.OnSelectedChanged += OnSelectedChanged;
        }

        UpdateVisuals();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);

        if (gridFormationSystem != null)
        {
            gridFormationSystem.OnSelectedChanged -= OnSelectedChanged;
        }
    }

    private void Update()
    {
        UpdateButtonState();
    }

    private void OnButtonClicked()
    {
        if (gridFormationSystem == null || unitData == null)
        {
            return;
        }

        if (gridFormationSystem.GetPlacedObjectTypeSO() == unitData)
        {
            gridFormationSystem.DeselectObjectType();
        }
        else
        {
            gridFormationSystem.SelectObjectType(unitData);
        }
    }

    private void OnSelectedChanged(object sender, System.EventArgs e)
    {
        UpdateButtonState();
    }

    private void UpdateVisuals()
    {
        if (unitData == null)
        {
            return;
        }

        if (iconImage != null && unitData.UnitIcon != null)
        {
            iconImage.sprite = unitData.UnitIcon;
        }

        if (unitNameText != null)
        {
            unitNameText.text = unitData.UnitName;
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (button == null)
        {
            return;
        }

        bool canUseBuild = CombatManager.Instance == null || !CombatManager.Instance.IsCombatActive;
        bool isSelected = gridFormationSystem != null && unitData != null && gridFormationSystem.GetPlacedObjectTypeSO() == unitData;

        button.interactable = canUseBuild;

        if (iconImage != null)
        {
            if (!canUseBuild)
            {
                iconImage.color = disabledColor;
            }
            else if (isSelected)
            {
                iconImage.color = selectedColor;
            }
            else
            {
                iconImage.color = normalColor;
            }
        }

        if (selectionHighlight != null)
        {
            selectionHighlight.gameObject.SetActive(isSelected && canUseBuild);
        }
    }

    public void SetUnitData(UnitDataSO newUnitData)
    {
        unitData = newUnitData;
        UpdateVisuals();
    }
}
