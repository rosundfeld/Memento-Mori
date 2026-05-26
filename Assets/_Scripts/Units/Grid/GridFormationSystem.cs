using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;

using UnityEngine.InputSystem;

public class GridFormationSystem : MonoBehaviour
{

    public static GridFormationSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;


    private Grid<GridObject> grid;
    [SerializeField] private List<UnitDataSO> placedObjectTypeSOList = null;
    private UnitDataSO placedObjectTypeSO;
    private UnitDataSO.Dir dir;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 originPosition;

    private void Awake()
    {
        Instance = this;

        int gridWidth = this.gridWidth;
        int gridHeight = this.gridHeight;
        float cellSize = this.cellSize;
        Vector3 originPosition = this.originPosition;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, originPosition, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));

        placedObjectTypeSO = null;
    }

    public class GridObject
    {

        private Grid<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject_Done placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && IsClickOnTaggedUI())
        {
            DeselectObjectType();
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && placedObjectTypeSO != null)
        {
            if (grid == null)
            {
                Debug.LogError("GridFormationSystem: grid is null!");
                return;
            }

            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int z);

            // Valida se a posição inicial está dentro do grid
            if (x < 0 || z < 0 || x >= grid.GetWidth() || z >= grid.GetHeight())
            {
                Debug.LogWarning($"GridFormationSystem: Click position ({x}, {z}) is outside grid bounds!");
                return;
            }

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);

            // Test Can Build
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                GridObject gridObject = grid.GetGridObject(gridPosition.x, gridPosition.y);
                if (gridObject == null)
                {
                    Debug.LogWarning($"GridFormationSystem: GridObject is null at position ({gridPosition.x}, {gridPosition.y})");
                    canBuild = false;
                    break;
                }

                if (!gridObject.CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

                PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                if (placedObject == null)
                {
                    Debug.LogError("GridFormationSystem: Falha ao criar PlacedObject_Done. Verifique logs anteriores.");
                    return;
                }
                placedObject.transform.rotation = Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }

                OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                //DeselectObjectType();
            }
            else
            {
                // Cannot build here
                UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            dir = UnitDataSO.GetNextDir(dir);
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) { SelectObjectTypeByIndex(0); }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { SelectObjectTypeByIndex(1); }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { SelectObjectTypeByIndex(2); }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) { SelectObjectTypeByIndex(3); }
        if (Keyboard.current.digit5Key.wasPressedThisFrame) { SelectObjectTypeByIndex(4); }
        if (Keyboard.current.digit6Key.wasPressedThisFrame) { SelectObjectTypeByIndex(5); }

        if (Keyboard.current.digit0Key.wasPressedThisFrame) { DeselectObjectType(); }


        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null)
            {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }

    public void DeselectObjectType()
    {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    public void SelectObjectType(UnitDataSO unitData)
    {
        if (unitData == null)
        {
            DeselectObjectType();
            return;
        }

        placedObjectTypeSO = unitData;
        RefreshSelectedObjectType();
    }

    public void SelectObjectTypeByIndex(int index)
    {
        if (placedObjectTypeSOList == null || index < 0 || index >= placedObjectTypeSOList.Count)
        {
            Debug.LogWarning($"GridFormationSystem: invalid unit index {index}");
            return;
        }

        SelectObjectType(placedObjectTypeSOList[index]);
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool IsClickOnTaggedUI()
    {
        if (EventSystem.current == null || Mouse.current == null)
        {
            return false;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject != null && result.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        grid.GetXY(mousePosition, out int x, out int y);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public UnitDataSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    /// <summary>
    /// Converte posição do mundo para célula do grid (alias para GetGridPosition)
    /// </summary>
    public Vector2Int GetCellFromWorldPosition(Vector3 worldPosition)
    {
        return GetGridPosition(worldPosition);
    }

    /// <summary>
    /// Retorna a Unit que está na célula especificada (se houver)
    /// </summary>
    public Unit GetUnitAtCell(Vector2Int cell)
    {
        if (grid == null) return null;

        GridObject gridObject = grid.GetGridObject(cell.x, cell.y);
        if (gridObject == null || gridObject.GetPlacedObject() == null) return null;

        // Tenta pegar o componente Unit do PlacedObject
        return gridObject.GetPlacedObject().GetComponent<Unit>();
    }

}
