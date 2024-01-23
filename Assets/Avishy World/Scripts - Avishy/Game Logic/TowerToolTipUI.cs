using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerToolTipUI : MonoBehaviour
{
    [SerializeField] private Transform objectFocus;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Transform buffContentHolder;
    [SerializeField] private GameObject buffHolderUIPrefab;


    private GridCell currentCellSelected;
    private void OnDisable()
    {
        foreach (Transform child in buffContentHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }




    public void DisplayTowerBuffs(TowerBaseParent tower)
    {
        if (!GameGridControls.Instance.ReturnCurrentCell()) return; // if no cell

        currentCellSelected = GameGridControls.Instance.ReturnCurrentCell();

        if (!currentCellSelected.ReturnIsOccipiedByTower()) return; //if cell has no tower on it

        objectFocus = currentCellSelected.transform;

        Vector3 pos;
        pos = Camera.main.WorldToScreenPoint(objectFocus.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), pos, Camera.main, out localPoint);
        transform.localPosition = localPoint + offset;

        foreach (TowerBuffDataHolder data in tower.ReturnTowerBuffList())
        {
            GameObject buffHolderObject = Instantiate(buffHolderUIPrefab, buffContentHolder);
            BuffHolderUI buffHolderUI;

            buffHolderObject.TryGetComponent<BuffHolderUI>(out buffHolderUI);
            if(buffHolderUI)
            {
                buffHolderUI.InitData(data.bgColor, data.buffIconSprite, data.amount.ToString());
            }
        }
    }
}
