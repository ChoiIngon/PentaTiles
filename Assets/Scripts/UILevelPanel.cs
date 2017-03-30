using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelPanel : MonoBehaviour {
	private static UILevelPanel _instance;  
	public static UILevelPanel Instance {  
		get {  
			if (!_instance) {  
				_instance = (UILevelPanel)GameObject.FindObjectOfType(typeof(UILevelPanel));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "UILevelPanel";  
					_instance = container.AddComponent<UILevelPanel>();  
				}  
			}  

			return _instance;  
		}  
	}
	public UIStartLevel startLevelPrefab;
	public Transform content;
	// Use this for initialization
	void Start () {
		content = transform.FindChild ("Viewport/Content");

		for (int i = 0; i < 10; i++) {
			UIStartLevel startLevel = GameObject.Instantiate<UIStartLevel> (startLevelPrefab);
			startLevel.transform.SetParent (content, false);
			startLevel.Init (i + 1, "level title" + (i+1).ToString());
		}

		Game.Instance.gameObject.SetActive (false);
	}
}
