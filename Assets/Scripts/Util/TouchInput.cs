using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {
	public delegate void OnTouchDownDelegate(Vector3 position);
	public delegate void OnTouchUpDelegate(Vector3 position);
	public delegate void OnTouchDragDelegate(Vector3 position);
    public OnTouchDownDelegate onTouchDown;
    public OnTouchUpDelegate onTouchUp;
    public OnTouchDragDelegate onTouchDrag;

	private Vector3 lastPressPosition;
	private bool isButtonDown;
	private float distanceFromCamera;
    void Start()
    {
		lastPressPosition = Input.mousePosition;
		isButtonDown = false;
		distanceFromCamera = 0.0f;
	}

	void OnMouseDown() {
		if (!enabled) 
			return;
		isButtonDown = true;

		distanceFromCamera = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
		lastPressPosition = Camera.main.ScreenToWorldPoint (
			//new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera)
		);
		if (null != onTouchDown)
		{
			onTouchDown(lastPressPosition);
		}
	}
	void OnMouseDrag() {
		if (!enabled || false == isButtonDown) 
			return;
		Vector3 currentPressPosition = Camera.main.ScreenToWorldPoint (
			//new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera)
		);
		if(null != onTouchDrag && currentPressPosition != lastPressPosition)
		{
			onTouchDrag(currentPressPosition - lastPressPosition);
		}
		lastPressPosition = currentPressPosition;
	}
	void OnMouseUp() {
		if (!enabled || false == isButtonDown) 
			return;
		isButtonDown = false;
		if (null != onTouchUp)
		{
			onTouchUp(lastPressPosition);
		}
	}
}
