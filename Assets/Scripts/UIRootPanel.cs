using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRootPanel : MonoBehaviour {
	public float screenWidth;
	public float scrollTime;
	public RectTransform rectTransform;
	private Coroutine scrollScreenCoroutine;

	// Use this for initialization
	void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
		scrollScreenCoroutine = null;
	}

	public void ScrollScreen(float direction)
	{
		if (null != scrollScreenCoroutine) {
			return;	
		}

		scrollScreenCoroutine = StartCoroutine (_ScrollScreen (direction));
	}

	private IEnumerator _ScrollScreen(float direction)
	{
		float moveDistance = Mathf.Abs(screenWidth * direction);
		Vector2 position = rectTransform.anchoredPosition;
		float originalPosition = position.x;
		while (0.0f < moveDistance) {
			float delta = screenWidth * Time.deltaTime / scrollTime;
			position.x += delta * direction;
			rectTransform.anchoredPosition = position;
			moveDistance -= delta;
			yield return null;
		}

		rectTransform.anchoredPosition = new Vector2 (originalPosition + screenWidth * direction, rectTransform.anchoredPosition.y);
		scrollScreenCoroutine = null;
	}
}
