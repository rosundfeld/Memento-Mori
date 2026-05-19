using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhost2D : MonoBehaviour {

    private Transform visual;
    private UnitDataSO placedObjectTypeSO;

    private void Start() {
        RefreshVisual();

        GridFormationSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = GridFormationSystem.Instance.GetMouseWorldSnappedPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, GridFormationSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        UnitDataSO placedObjectTypeSO = GridFormationSystem.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.UnitVisual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
        }
    }

}
