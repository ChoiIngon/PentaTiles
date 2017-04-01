using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	[System.Serializable]
	public class StageData {
		public int stage;
		public int level;
	}
	[System.Serializable]
	public class PlayData
	{
		public UIStageInfo.Info current_stage;
		public int current_level;
		public StageData[] stageDatas;
		public int hint;
	}
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
	public UIGamePanel gamePanel;

	public float scrollTime;
	public CanvasScaler canvasScaler;
	public PlayData playData;
	//public GameObject background;

	private float uiWidth;
	void Start()
	{
		//iTween.ShakePosition (background, new Vector3 (3.0f, 3.0f, 3.0f), 60.0f);
		uiWidth = canvasScaler.referenceResolution.x;
		Map.Instance.gameObject.SetActive (false);
	}

	public IEnumerator CheckCompleteStage() {
		if (true == Map.Instance.CheckComplete ()) {
			yield return StartCoroutine(gamePanel.resultPanel.Activate ());
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

	public void StartLevel(int stage, int level)
	{
		gamePanel.level.text = "Level - " + level;
		Map.Instance.gameObject.SetActive(true);
		Map.Instance.Init(stage, level);
	}
}
