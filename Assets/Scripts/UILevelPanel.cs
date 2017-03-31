using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelPanel : MonoBehaviour {
	public UILevelInfo levelInfoPrefab;
	public Transform gridView;
	// Use this for initialization
	public void Init (int stage) {
		while (0 < gridView.childCount) {
			Transform child = gridView.GetChild (0);
			child.SetParent (null);
			DestroyImmediate (child.gameObject);
		}
		for (int i = 0; i < 24; i++) {
			UILevelInfo levelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			levelInfo.transform.SetParent (gridView, false);
			levelInfo.Init (stage, i + 1);
		}
	}

}
