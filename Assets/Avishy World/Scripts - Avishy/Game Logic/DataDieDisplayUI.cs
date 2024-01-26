using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DataDieDisplayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image connectedImage;
    [SerializeField] DieData connectedData;

    private void Start()
    {
        connectedImage = GetComponent<Image>();
    }

    public void InitDisplay(DieData data)
    {
        connectedData = data;
        connectedImage.sprite = data.dieIcon;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.DisplayDiceFacesUI(true, connectedData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.DisplayDiceFacesUI(false, connectedData);
    }
}
