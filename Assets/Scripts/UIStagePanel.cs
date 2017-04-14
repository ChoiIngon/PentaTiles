using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStagePanel : MonoBehaviour {
	public UIStageInfo stageInfoPrefab;
    public ScrollRect stageScrollRectPrefab;

    public ScrollSnapRect scrollSnapRect;
    public List<ScrollRect> stageScrollRects;
	public List<UIStageInfo> stageInfos;
	public Transform content;
	public Text totalStarCountText;

	public int totalStarCount {
		set {
			totalStarCountText.text = value.ToString ();
		}
	}
	// Use this for initialization
	public void Init () {
		stageScrollRects = new List<ScrollRect>();
		stageScrollRects.AddRange (new ScrollRect[Game.Instance.config.worldInfos.Count]);
		stageInfos = new List<UIStageInfo> ();
		stageInfos.AddRange (new UIStageInfo[Game.Instance.config.stageInfos.Count]);

		totalStarCount = Game.Instance.playData.star;

		for (int i = 0; i < stageScrollRects.Count; i++) {
			ScrollRect scrollRect = GameObject.Instantiate<ScrollRect> (stageScrollRectPrefab);
			scrollRect.transform.SetParent (content, false);
			stageScrollRects[i] = scrollRect;
		}

		foreach(Config.WorldInfo worldInfo in Game.Instance.config.worldInfos)
		{
			ScrollRect stageScrollRect = stageScrollRects[worldInfo.id - 1];
			foreach(Config.StageInfo stageInfo in worldInfo.stageInfos)
			{
				UIStageInfo uiStageInfo = GameObject.Instantiate<UIStageInfo>(stageInfoPrefab);
				uiStageInfo.transform.SetParent(stageScrollRect.content, false);
				uiStageInfo.Init(stageInfo);
				stageInfos [stageInfo.id - 1] = uiStageInfo;
            }
		}
        scrollSnapRect.Init();
        Map.Instance.gameObject.SetActive (false);
	}

	public UIStageInfo GetStageInfo(int stage)
	{
		if (0 >= stage || stageInfos.Count < stage) {
			throw new System.Exception ("invalid stage id:" + stage);
		}
		return stageInfos [stage - 1];
	}
}
