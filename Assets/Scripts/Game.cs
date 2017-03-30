using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public UIStagePanel stagePanel;
	public UILevelPanel levelPanel;
	public UIResultPanel resultPanel;

	void Start()
	{
		stagePanel.gameObject.SetActive (true);
		levelPanel.gameObject.SetActive (false);
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
}
