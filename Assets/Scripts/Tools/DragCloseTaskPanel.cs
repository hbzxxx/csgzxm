using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCloseTaskPanel : MonoBehaviour, IBeginDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<ScrollRect>().OnBeginDrag(eventData);
        PanelManager.Instance.CloseTaskGuidePanel();
    }
}
