using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartCombatButton : MonoBehaviour
{
    [SerializeField] private Text buttonText; // Opcional: para mudar o texto do botão
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        UpdateButtonState();
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }

    void Update()
    {
        UpdateButtonState();
    }

    private void OnButtonClicked()
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.StartCombat();
        }
        else
        {
            Debug.LogError("StartCombatButton: CombatManager.Instance not found! Button will remain interactable.");
        }
    }

    private void UpdateButtonState()
    {
        if (button == null) return;

        // Se o CombatManager não existir ainda, garante que o botão esteja visível/ativado
        if (CombatManager.Instance == null)
        {
            if (!button.interactable)
            {
                Debug.Log("StartCombatButton: CombatManager missing - forcing button.interactable = true");
            }
            button.interactable = true;
            if (buttonText != null) buttonText.text = "Start Combat";
            return;
        }

        // Caso o CombatManager exista, use o estado dele
        bool shouldBeInteractable = !CombatManager.Instance.IsCombatActive;
        if (button.interactable != shouldBeInteractable)
        {
            // Só loga mudanças de estado para evitar poluição do console
            Debug.Log($"StartCombatButton: setting interactable={shouldBeInteractable} (combatActive={CombatManager.Instance.IsCombatActive})");
        }
        button.interactable = shouldBeInteractable;

        // Atualiza o texto do botão (opcional)
        if (buttonText != null)
        {
            buttonText.text = CombatManager.Instance.IsCombatActive ? "Combat Active" : "Start Combat";
        }
    }
}
