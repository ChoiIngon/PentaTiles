using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragEventForwarder : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public enum ScrollDirection
	{
		Vertical,
		Horizontal
	}
	ScrollDirection scrollDirection;
	ScrollSnapRect scrollRectSnap;
    private void Start()
    {
        scrollRectSnap = GetComponentInParent<ScrollSnapRect>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
		scrollDirection = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) ? ScrollDirection.Horizontal : ScrollDirection.Vertical;
		if(null != scrollRectSnap && ScrollDirection.Horizontal == scrollDirection)
        {
            scrollRectSnap.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
		if(null != scrollRectSnap && ScrollDirection.Horizontal == scrollDirection)
        {
            scrollRectSnap.OnDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
		if(null != scrollRectSnap && ScrollDirection.Horizontal == scrollDirection)
        {
            scrollRectSnap.OnEndDrag(eventData);
        }
    }
}