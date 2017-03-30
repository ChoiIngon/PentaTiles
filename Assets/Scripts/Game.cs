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

	public Map map;
	public UIResultPanel resultPanel;

	void Start()
	{
		if (null != resultPanel) {
			resultPanel.gameObject.SetActive (false);
		}
	}

	public IEnumerator CheckCompleteStage() {
		if (true == map.CheckComplete ()) {
			yield return StartCoroutine(resultPanel.Activate ());
			//if (true == resultPanel.isNext) {

			//}
			map.Init (null);
		}
		yield break;
	}
}
