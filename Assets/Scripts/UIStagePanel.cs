using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStagePanel : MonoBehaviour {
	public UIStageInfo stageInfoPrefab;
	public Transform content;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++) {
			UIStageInfo stageInfo = GameObject.Instantiate<UIStageInfo> (stageInfoPrefab);
			stageInfo.transform.SetParent (content, false);
			stageInfo.Init (i + 1, "stage title" + (i+1).ToString());
		}

		Map.Instance.gameObject.SetActive (false);
	}
}
