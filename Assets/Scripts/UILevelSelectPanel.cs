using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelectPanel : MonoBehaviour {
	private const int MAX_LEVEL = 24;
	private Config.StageInfo stageInfo;
	public UILevelInfo levelInfoPrefab;
	public List<UILevelInfo> uiLevelInfos;
	public Transform gridView;

	public void Init () {
		uiLevelInfos = new List<UILevelInfo> ();
		for (int i = 0; i < MAX_LEVEL; i++) {
			UILevelInfo uiLevelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			uiLevelInfo.transform.SetParent (gridView, false);
			uiLevelInfos.Add (uiLevelInfo);
		}
	}

	public void SetLevelInfos(Config.StageInfo info) {
		Game.Instance.playData.currentStage = info.id;
		stageInfo = info;
		for (int i = 0; i < stageInfo.totalLevel; i++) {
			UILevelInfo uiLevelInfo = uiLevelInfos [i];
			uiLevelInfo.Init (info.id, i + 1);
		}
	}
	public UILevelInfo GetLevelInfo(int level) {
		if (0 >= level || stageInfo.totalLevel < level) {
			throw new System.Exception ("invalid level id:" + level);
		}
		return uiLevelInfos [level - 1];
	}
}
