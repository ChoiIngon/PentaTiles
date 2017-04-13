using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStagePanel : MonoBehaviour {
	public UIStageInfo stageInfoPrefab;
    public ScrollRect stagePagePrefab;

    public ScrollSnapRect scrollSnapRect;
    public List<ScrollRect> stagePages;
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
        stagePages = new List<ScrollRect>();
        stageInfos = new List<UIStageInfo> ();
		totalStarCount = Game.Instance.playData.star;

		foreach(UIStageInfo.Info info in Game.Instance.stageInfos.stage_infos)
		{
            if (info.page > stagePages.Count)
            {
                ScrollRect scrollRect = GameObject.Instantiate<ScrollRect>(stagePagePrefab);
                scrollRect.transform.SetParent(content, false);
                stagePages.Add(scrollRect);
            }

            {
                ScrollRect scrollRect = stagePages[info.page - 1];
                UIStageInfo stageInfo = GameObject.Instantiate<UIStageInfo>(stageInfoPrefab);
                stageInfo.transform.SetParent(scrollRect.content, false);
                stageInfo.Init(info);
                stageInfos.Add(stageInfo);
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
