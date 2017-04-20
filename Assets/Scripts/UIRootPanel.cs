using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRootPanel : MonoBehaviour {
    private CanvasScaler canvasScaler;
	public float scrollTime;
	public RectTransform rectTransform;
	private Coroutine scrollScreenCoroutine;

	// Use this for initialization
	void Awake()
	{
        canvasScaler = GetComponentInParent<CanvasScaler>();
        rectTransform = GetComponent<RectTransform> ();
		scrollScreenCoroutine = null;
	}

	public void ScrollScreen(Vector3 direction)
	{
		if (null != scrollScreenCoroutine) {
			return;	
		}

		scrollScreenCoroutine = StartCoroutine (_ScrollScreen (direction));
	}

	private IEnumerator _ScrollScreen(Vector3 direction)
	{
        Vector2 src = rectTransform.anchoredPosition;
        Vector2 dest = new Vector2(rectTransform.anchoredPosition.x + canvasScaler.referenceResolution.x * direction.x, rectTransform.anchoredPosition.y + canvasScaler.referenceResolution.y * direction.y);

        float interpolate = 0.0f;
        while (1.0 > interpolate) {
            rectTransform.anchoredPosition = Vector2.Lerp(src, dest, interpolate);
            interpolate += Time.deltaTime / scrollTime;
            yield return null;
		}

        rectTransform.anchoredPosition = dest;
        scrollScreenCoroutine = null;
	}
}
