using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private bool isPaused = false;

    [field: SerializeField]
    public bool IsCombatActive { get; private set; }

    void Awake()
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

        // Inicia em fase de planejamento (combate inativo)
        IsCombatActive = false;
    }

    void Update()
    {
        // // Opcional: atalho para testar (remova depois se preferir)
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     ToggleCombat();
        // }
    }

    public void StartCombat()
    {
        IsCombatActive = true;
        Debug.Log("Combat started! Units can now attack.");
    }

    public void EndCombat()
    {
        IsCombatActive = false;
        Debug.Log("Combat ended! Back to planning phase.");
    }

    public void ToggleCombat()
    {
        if (IsCombatActive)
            EndCombat();
        else
            StartCombat();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
