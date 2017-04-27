using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementPanel : MonoBehaviour {
    public UIAchievementInfo achievementInfoPrefab;
    public RectTransform outerWindow;
    public Transform content;
	public Button backButton;
    private List<UIAchievementInfo> uiAchievementInfos;
    private List<Achievement> achievements;

    public void Init()
	{
		uiAchievementInfos = new List<UIAchievementInfo>();
        achievements = new List<Achievement>();
        foreach (var itr in Game.Instance.playData.achievements)
        {
            Achievement achievement = itr.Value;
            achievements.Add(achievement);

            UIAchievementInfo uiAchievementInfo = GameObject.Instantiate<UIAchievementInfo>(achievementInfoPrefab);
            uiAchievementInfo.Init(achievement);
            uiAchievementInfo.transform.SetParent(content, false);
            uiAchievementInfos.Add(uiAchievementInfo);
        }
		backButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.stagePanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, 1.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
		});
		gameObject.SetActive (false);
    }

	public void Sort()
	{
		achievements.Sort (Achievement.Compare);
		for(int i=0; i<achievements.Count; i++)
        {
            uiAchievementInfos[i].Init(achievements[i]);
		}
	}
}
