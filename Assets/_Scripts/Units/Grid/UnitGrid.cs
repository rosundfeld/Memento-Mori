using UnityEngine;

public class UnitGrid
{
    private int width;
    private int height;
    private Vector3 originPosition;
    private float cellSize;
    private int[,] GridArray;

    private TextMesh[,] debugTextArray;
    private Transform debugParent;

    public UnitGrid(int width, int height, float cellSize, Vector3 originPosition, Transform parent = null)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.debugParent = parent;
        GridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        // Cria os textos de debug
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                debugTextArray[x, y] = CreateWorldText(
                    GridArray[x, y].ToString(), 
                    debugParent, 
                    GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 
                    20, 
                    Color.white
                );
            }
        }
    }

    // Método para desenhar o grid - deve ser chamado de OnDrawGizmos ou Update
    public void DrawDebug()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white);
    }

    private TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, int fontSize, Color color)
    {
        GameObject go = new GameObject("World_Text", typeof(TextMesh));
        Transform t = go.transform;
        if (parent != null)
            t.SetParent(parent, false);
        t.position = localPosition;

        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = color;
        textMesh.characterSize = 0.1f;

        return textMesh;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            GridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
        y = Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellSize);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return GridArray[x, y];
        }
        else
        {
            return -1;
        }
    }
    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}
