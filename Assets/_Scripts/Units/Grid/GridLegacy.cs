using UnityEngine;

public class GridLegacy : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 originPosition = Vector3.zero;
    [SerializeField] private bool showDebugText = true;
    [SerializeField] private bool showDebugLines = true;

    private UnitGrid grid;

    private void Awake()
    {
        grid = new UnitGrid(gridWidth, gridHeight, cellSize, originPosition, showDebugText ? transform : null);
    }

    private void Update()
    {
        // Desenha as linhas do grid a cada frame para visualização
        if (showDebugLines && grid != null)
        {
            grid.DrawDebug();
        }
    }

    private void OnDrawGizmos()
    {
        // Desenha o grid no editor mesmo sem estar em play mode
        if (grid == null) return;

        Gizmos.color = Color.green;
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = originPosition + new Vector3(x * cellSize, 0, 0);
            Vector3 end = originPosition + new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = originPosition + new Vector3(0, y * cellSize, 0);
            Vector3 end = originPosition + new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    // Métodos públicos para interagir com o grid
    public void SetGridValue(int x, int y, int value)
    {
        grid?.SetValue(x, y, value);
    }

    public void SetGridValue(Vector3 worldPosition, int value)
    {
        grid?.SetValue(worldPosition, value);
    }

    public int GetGridValue(int x, int y)
    {
        return grid?.GetValue(x, y) ?? -1;
    }

    public int GetGridValue(Vector3 worldPosition)
    {
        return grid?.GetValue(worldPosition) ?? -1;
    }
}
