using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerToolTipUI : MonoBehaviour
{
    [SerializeField] private Transform objectFocus;
    [SerializeField] private Vector2 offset;

    void Update()
    {
        if (!GameGridControls.Instance.ReturnCurrentCell()) return; //temp

        objectFocus = GameGridControls.Instance.ReturnCurrentCell().transform;

        Vector3 pos;
        pos = Camera.main.WorldToScreenPoint(objectFocus.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), pos, Camera.main, out localPoint);
        transform.localPosition = localPoint + offset;
    }
}
