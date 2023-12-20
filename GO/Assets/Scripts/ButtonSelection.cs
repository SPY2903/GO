using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int value;
    private SelectionControl selectionControl;
    private void Start()
    {
        selectionControl = GameObject.FindGameObjectWithTag("SelectionControl").GetComponent<SelectionControl>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectionControl.AccessSelection();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectionControl.SetCurrentSelection(value);
        selectionControl.SetSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
