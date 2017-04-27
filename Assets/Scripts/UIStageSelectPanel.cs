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
	public Button shopButton;
	public GameObject newAchievement;

	public int totalStarCount {
		set {
			totalStarCountText.text = value.ToString ();
		}
	}

	public void Init () {
        totalStarCount = Game.Instance.playData.star;

        stageScrollRects = new List<ScrollRect>(new ScrollRect[Game.Instance.config.worldInfos.Count]);
		stageInfos = new List<UIStageInfo> (new UIStageInfo[Game.Instance.config.stageInfos.Count]);
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

		achievementButton.onClick.AddListener(() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.achievementPanel.gameObject.SetActive(true);
			Game.Instance.achievementPanel.Sort();
			Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, -1.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
			newAchievement.SetActive (false);
		});

		shopButton.onClick.AddListener(() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.shopPanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, 1.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
		});
	}

	public UIStageInfo GetStageInfo(int stage)
	{
		if (0 >= stage || stageInfos.Count < stage) {
			throw new System.Exception ("invalid stage id:" + stage);
		}
		return stageInfos [stage - 1];
	}
}
