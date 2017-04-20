using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSelectPanel : MonoBehaviour {
	public UIStageInfo stageInfoPrefab;
    public ScrollRect stageScrollRectPrefab;

    public ScrollSnapRect scrollSnapRect;
    public List<ScrollRect> stageScrollRects;
	public List<UIStageInfo> stageInfos;
	public Transform content;
	public Text totalStarCountText;
    public Button achievementButton;


	public int totalStarCount {
		set {
			totalStarCountText.text = value.ToString ();
		}
	}

    private void Awake()
    {
        achievementButton.onClick.AddListener(() =>
        {
            Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, -1.0f, 0.0f));
        });
    }
    public void Init () {
		stageScrollRects = new List<ScrollRect>();
		stageScrollRects.AddRange (new ScrollRect[Game.Instance.config.worldInfos.Count]);
		stageInfos = new List<UIStageInfo> ();
		stageInfos.AddRange (new UIStageInfo[Game.Instance.config.stageInfos.Count]);

		totalStarCount = Game.Instance.playData.star;

		for (int i = 0; i < Game.Instance.config.worldInfos.Count; i++) {
			ScrollRect stageScrollRect = GameObject.Instantiate<ScrollRect> (stageScrollRectPrefab);
			stageScrollRect.transform.SetParent (content, false);
			stageScrollRects[i] = stageScrollRect;

			Config.WorldInfo worldInfo = Game.Instance.config.worldInfos[i];
			foreach(Config.StageInfo stageInfo in worldInfo.stageInfos)
			{
				UIStageInfo uiStageInfo = GameObject.Instantiate<UIStageInfo>(stageInfoPrefab);
				uiStageInfo.transform.SetParent(stageScrollRect.content, false);
				uiStageInfo.Init(stageInfo);
				stageInfos [stageInfo.id - 1] = uiStageInfo;
			}
		}
	
        scrollSnapRect.Init();
	}

	public UIStageInfo GetStageInfo(int stage)
	{
		if (0 >= stage || stageInfos.Count < stage) {
			throw new System.Exception ("invalid stage id:" + stage);
		}
		return stageInfos [stage - 1];
	}
}
