using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSelectPanel : MonoBehaviour {
	private const int MAX_LEVEL = 24;
	private Config.StageInfo stageInfo;
	public UILevelInfo levelInfoPrefab;
	public List<UILevelInfo> uiLevelInfos;
	public Transform gridView;
	public Button backButton; 

	public void Init () {
		uiLevelInfos = new List<UILevelInfo> ();
		for (int i = 0; i < MAX_LEVEL; i++) {
			UILevelInfo uiLevelInfo = GameObject.Instantiate<UILevelInfo> (levelInfoPrefab);
			uiLevelInfo.transform.SetParent (gridView, false);
			uiLevelInfos.Add (uiLevelInfo);
		}
		gameObject.SetActive (false);
		backButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.stagePanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(1.0f, 0.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
		});
	}

	void OnEnable()
	{
		if (0 >= Game.Instance.playData.currentStage || Game.Instance.playData.currentStage > Game.Instance.config.stageInfos.Count) {
			return;
		}
		stageInfo = Game.Instance.config.stageInfos [Game.Instance.playData.currentStage - 1];
		for (int i = 0; i < stageInfo.totalLevel; i++) {
			UILevelInfo uiLevelInfo = uiLevelInfos [i];
			uiLevelInfo.Init (stageInfo.id, i + 1);
		}
	}

	public UILevelInfo GetLevelInfo(int level) {
		if (0 >= level || stageInfo.totalLevel < level) {
			throw new System.Exception ("invalid level id:" + level);
		}
		return uiLevelInfos [level - 1];
	}
}
