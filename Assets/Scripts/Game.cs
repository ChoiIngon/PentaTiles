using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	private static Game _instance;  
	public static Game Instance {  
		get {  
			if (!_instance) {  
				_instance = (Game)GameObject.FindObjectOfType(typeof(Game));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "Game";  
					_instance = container.AddComponent<Game>();  
				}  
			}  

			return _instance;  
		}  
	}
		
	public RectTransform uiRoot;
	public UIStagePanel stagePanel;
	public UILevelPanel levelPanel;
	public UIResultPanel resultPanel;
	public float scrollTime;
	public CanvasScaler canvasScaler;
	//public GameObject background;

	private float uiWidth;
	void Start()
	{
		//iTween.ShakePosition (background, new Vector3 (3.0f, 3.0f, 3.0f), 60.0f);
		uiWidth = canvasScaler.referenceResolution.x;
		//stagePanel.gameObject.SetActive (true);
		//levelPanel.gameObject.SetActive (false);
		Map.Instance.gameObject.SetActive (false);
		resultPanel.gameObject.SetActive (false);
	}

	public IEnumerator CheckCompleteStage() {
		if (true == Map.Instance.CheckComplete ()) {
			yield return StartCoroutine(resultPanel.Activate ());
			//if (true == resultPanel.isNext) {

			//}
			Map.Instance.Init (null);
		}
		yield break;
	}
		
	Coroutine scrollScreenCoroutine;
	public void ScrollScreen(float direction)
	{
		if (null != scrollScreenCoroutine) {
			return;	
		}

		scrollScreenCoroutine = StartCoroutine (_ScrollScreen (direction));
	}
	private IEnumerator _ScrollScreen(float direction)
	{
		Debug.Log ("call ScrollScreen");

		float moveDistance = uiWidth;
		Vector2 position = uiRoot.anchoredPosition;
		float originalPosition = position.x;
		while (0.0f < moveDistance) {
			float delta = uiWidth * Time.deltaTime / scrollTime;
			position.x += delta * direction;
			uiRoot.anchoredPosition = position;
			moveDistance -= delta;
			yield return null;
		}

		uiRoot.anchoredPosition = new Vector2 (originalPosition + uiWidth * direction, uiRoot.anchoredPosition.y);

		scrollScreenCoroutine = null;
	}
}
