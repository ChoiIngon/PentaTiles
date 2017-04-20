using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementPanel : MonoBehaviour {
    public UIAchievementInfo achievementInfoPrefab;
    public RectTransform outerWindow;
    public Button backButton;
    private Vector2 targetPosition;
	public Transform content;
	public void Init()
	{
		backButton.onClick.AddListener(() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, 1.0f, 0.0f));
		});
	}
	public void Sort()
	{
		while(content.childCount > 0) {
			Transform child = content.GetChild (0);
			child.transform.SetParent (null);
			DestroyImmediate (child.gameObject);
		}
		List<Achievement> achievements = new List<Achievement> ();
		foreach (var itr in Game.Instance.playData.achievements) {
			Achievement achievement = itr.Value;
			achievements.Add (achievement);
		}
		achievements.Sort (Achievement.Compare);
		foreach (Achievement achievement in achievements) {
			UIAchievementInfo uiAchievementInfo = GameObject.Instantiate<UIAchievementInfo> (achievementInfoPrefab);
			uiAchievementInfo.Init (achievement);
			uiAchievementInfo.transform.SetParent (content, false);
		}
	}
}
