using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelPanel : MonoBehaviour {
	public UILevelInfo levelInfoPrefab;

	// Use this for initialization
	public void Init (int stage) {
		gameObject.SetActive(true);
		for (int i = 0; i < 20; i++) {
			UILevelInfo levelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			levelInfo.transform.SetParent (transform, false);
			levelInfo.Init (stage, i + 1);
			levelInfo.transform.SetParent (transform);
		}
	}

}
