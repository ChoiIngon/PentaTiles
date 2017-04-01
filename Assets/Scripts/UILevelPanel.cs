using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelPanel : MonoBehaviour {
	public UILevelInfo levelInfoPrefab;
	public List<UILevelInfo> levelInfos;
	public Transform gridView;
	// Use this for initialization
	public void Init (UIStageInfo.Info info) {
		while (0 < gridView.childCount) {
			Transform child = gridView.GetChild (0);
			child.SetParent (null);
			DestroyImmediate (child.gameObject);
		}

		PlayData.StageData stageData = Game.Instance.playData.stageDatas [info.stage - 1];

		levelInfos = new List<UILevelInfo> ();
		for (int i = 0; i < info.total_level; i++) {
			UILevelInfo levelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			levelInfo.transform.SetParent (gridView, false);
			levelInfo.Init (info.stage, i + 1);
			if (i <= stageData.level) {
				levelInfo.Unlock ();
			}
			levelInfos.Add (levelInfo);
		}
	}

	public UILevelInfo GetLevelInfo(int level) {
		if (0 >= level || levelInfos.Count < level) {
			throw new System.Exception ("invalid level id:" + level);
		}
		return levelInfos [level - 1];
	}
}
