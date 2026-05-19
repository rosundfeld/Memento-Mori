using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject_Done : MonoBehaviour
{

    public static PlacedObject_Done Create(Vector3 worldPosition, Vector2Int origin, UnitDataSO.Dir dir, UnitDataSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
        placedObject.Setup(placedObjectTypeSO, origin, dir);

        return placedObject;
    }


    private UnitDataSO placedObjectTypeSO;
    private Vector2Int origin;
    private UnitDataSO.Dir dir;
    private void Setup(UnitDataSO placedObjectTypeSO, Vector2Int origin, UnitDataSO.Dir dir)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.origin = origin;
        this.dir = dir;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return placedObjectTypeSO.UnitName;
    }

}
