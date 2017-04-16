using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelectPanel : MonoBehaviour {
	public UILevelInfo levelInfoPrefab;
	public List<UILevelInfo> levelInfos;
	public Transform gridView;
	// Use this for initialization
	public void Init (Config.StageInfo info) {
		while (0 < gridView.childCount) {
			Transform child = gridView.GetChild (0);
			child.SetParent (null);
			DestroyImmediate (child.gameObject);
		}

		PlayData.StageData stageData = Game.Instance.playData.stageDatas [info.id - 1];

		levelInfos = new List<UILevelInfo> ();
		for (int i = 0; i < info.totalLevel; i++) {
			UILevelInfo levelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			levelInfo.transform.SetParent (gridView, false);
			levelInfo.Init (info.id, i + 1);
			if (i <= stageData.clearLevel) {
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
