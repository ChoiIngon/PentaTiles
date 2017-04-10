using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragEventForwarder : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    ScrollSnapRect scrollRectSnap;

    private void Start()
    {
        scrollRectSnap = GetComponentInParent<ScrollSnapRect>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(null != scrollRectSnap)
        {
            scrollRectSnap.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (null != scrollRectSnap)
        {
            scrollRectSnap.OnDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (null != scrollRectSnap)
        {
            scrollRectSnap.OnEndDrag(eventData);
        }
    }
}