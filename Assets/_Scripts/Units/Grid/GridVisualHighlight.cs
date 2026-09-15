using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de highlight visual para células do grid (padrão xadrez)
/// Desaparece automaticamente ao iniciar o combate
/// </summary>
public class GridVisualHighlight : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color color1 = new Color(0.9f, 0.9f, 0.9f, 0.3f); // Branco translúcido
    [SerializeField] private Color color2 = new Color(0.8f, 0.8f, 0.8f, 0.2f); // Cinza translúcido
    [SerializeField] private bool showDuringCombat = false;
    [SerializeField] private float cellPadding = 0.05f; // Espaço entre células (visual)
    [SerializeField] private string sortingLayerName = "UI"; // Nome do Sorting Layer
    [SerializeField] private int sortingOrder = 10;

    [Header("Grid Reference")]
    [SerializeField] private GridFormationSystem gridFormationSystem;

    private List<GameObject> highlightObjects = new List<GameObject>();
    private bool isInitialized = false;
    private bool isVisible = true;

    private void Start()
    {
        if (gridFormationSystem == null)
        {
            gridFormationSystem = GridFormationSystem.Instance;
        }

        if (gridFormationSystem == null)
        {
            Debug.LogWarning("GridVisualHighlight: GridFormationSystem não encontrado!");
            return;
        }

        // Aguarda um frame para garantir que o grid foi inicializado
        Invoke(nameof(InitializeHighlights), 0.1f);

        // Inscreve em eventos (opcional - para atualizar quando unidade é colocada)
        if (gridFormationSystem != null)
        {
            gridFormationSystem.OnObjectPlaced += OnObjectPlaced;
        }
    }

    private void OnDestroy()
    {
        if (gridFormationSystem != null)
        {
            gridFormationSystem.OnObjectPlaced -= OnObjectPlaced;
        }

        // Limpa todos os highlights
        foreach (GameObject highlight in highlightObjects)
        {
            if (highlight != null)
            {
                Destroy(highlight);
            }
        }
        highlightObjects.Clear();
    }

    private void InitializeHighlights()
    {
        var gridInfo = gridFormationSystem.GetGridInfo();

        if (gridInfo.width <= 0 || gridInfo.height <= 0)
        {
            Debug.LogError("GridVisualHighlight: Grid info inválido!");
            return;
        }

        CreateHighlights(gridInfo);
        isInitialized = true;
    }

    private void CreateHighlights(GridFormationSystem.GridInfo gridInfo)
    {
        // Cria um GameObject pai para organizar
        GameObject highlightParent = new GameObject("GridHighlights");
        highlightParent.transform.SetParent(transform);

        for (int x = 0; x < gridInfo.width; x++)
        {
            for (int y = 0; y < gridInfo.height; y++)
            {
                // Cria célula visual
                GameObject cellHighlight = CreateCellHighlight(x, y, gridInfo);
                cellHighlight.transform.SetParent(highlightParent.transform);
                highlightObjects.Add(cellHighlight);

                // Alterna cor (padrão xadrez)
                SpriteRenderer spriteRenderer = cellHighlight.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = (x + y) % 2 == 0 ? color1 : color2;
                }
            }
        }

        Debug.Log($"GridVisualHighlight: Criados {highlightObjects.Count} highlights para grid {gridInfo.width}x{gridInfo.height}");
    }

    private GameObject CreateCellHighlight(int x, int y, GridFormationSystem.GridInfo gridInfo)
    {
        GameObject cellObj = new GameObject($"Cell_{x}_{y}");
        
        // Adiciona SpriteRenderer
        SpriteRenderer spriteRenderer = cellObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        
        // Configura Sorting Layer e Order
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
        
        // Posiciona e escala
        Vector3 worldPos = gridFormationSystem.GetCellWorldPosition(x, y);
        cellObj.transform.position = worldPos + new Vector3(gridInfo.cellSize * 0.5f, gridInfo.cellSize * 0.5f, 0); // Centro da célula
        
        // Aplica padding visual
        float visualSize = gridInfo.cellSize - (cellPadding * 2);
        cellObj.transform.localScale = new Vector3(visualSize, visualSize, 1f);

        return cellObj;
    }

    private Sprite CreateSquareSprite()
    {
        // Cria um sprite quadrado simples (1x1 pixel branco, esticado)
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Controla visibilidade baseado no estado do combate
        bool shouldShow = showDuringCombat || 
                         (CombatManager.Instance == null || !CombatManager.Instance.IsCombatActive);

        if (shouldShow != isVisible)
        {
            SetHighlightsVisible(shouldShow);
            isVisible = shouldShow;
        }
    }

    private void SetHighlightsVisible(bool visible)
    {
        foreach (GameObject highlight in highlightObjects)
        {
            if (highlight != null)
            {
                highlight.SetActive(visible);
            }
        }
    }

    private void OnObjectPlaced(object sender, System.EventArgs e)
    {
        // Opcional: você pode implementar lógica aqui para esconder highlights
        // em células ocupadas, se desejar
    }

    #region Public Methods

    /// <summary>
    /// Atualiza as cores do padrão xadrez
    /// </summary>
    public void UpdateColors(Color newColor1, Color newColor2)
    {
        color1 = newColor1;
        color2 = newColor2;

        if (!isInitialized || gridFormationSystem == null) return;

        var gridInfo = gridFormationSystem.GetGridInfo();
        int index = 0;
        
        foreach (GameObject highlight in highlightObjects)
        {
            if (highlight != null)
            {
                SpriteRenderer sr = highlight.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    int x = index % gridInfo.width;
                    int y = index / gridInfo.width;
                    sr.color = (x + y) % 2 == 0 ? color1 : color2;
                }
            }
            index++;
        }
    }

    /// <summary>
    /// Força mostrar/esconder highlights
    /// </summary>
    public void SetVisible(bool visible)
    {
        SetHighlightsVisible(visible);
        isVisible = visible;
    }

    /// <summary>
    /// Recria os highlights (útil se grid mudar)
    /// </summary>
    public void RefreshHighlights()
    {
        // Remove highlights antigos
        foreach (GameObject highlight in highlightObjects)
        {
            if (highlight != null)
            {
                Destroy(highlight);
            }
        }
        highlightObjects.Clear();

        // Recria
        if (gridFormationSystem != null)
        {
            InitializeHighlights();
        }
    }

    #endregion
}
