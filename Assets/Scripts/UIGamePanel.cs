using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class UIGamePanel : MonoBehaviour {
	public Text level;
	public Button backButton;
	public Button redoButton;
	public Button adsButton;
	public Button hintButton;
	public Text hintText;
	public UILevelComplete levelComplete;

	public int hintCount {
		set {
			if (99 < value) {
				hintText.text = "99+";
			} else {
				hintText.text = value.ToString ();
			}
		}
	}
	void Start () {
		backButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Map.Instance.gameObject.SetActive (false);
			levelComplete.gameObject.SetActive(false);
			Game.Instance.levelPanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(1.0f, 0.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
		});

        adsButton.onClick.AddListener (() => {
			//Game.Instance.unityAds.ShowRewardAds();
			Game.Instance.advertisement.Show(Advertisement.PlacementType.Rewarded, () =>
			{
				Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
					{ "stage", Game.Instance.playData.currentStage},
					{ "level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel},
				});
				Game.Instance.AddHint(Game.Instance.advertisement.reward_count);
				Quest.Update(Achievement.Type.AdsWatchCount, "");
			});
		});
		redoButton.onClick.AddListener (() => {
			Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel);
			AudioManager.Instance.Play("ButtonClick");
		});
		hintCount = Game.Instance.playData.hint;
		hintButton.onClick.AddListener (() => {
			if(false == Game.Instance.UseHint())
			{
				AudioManager.Instance.Play("BlockOut");
				iTween.ShakeRotation(hintButton.gameObject, new Vector3(0.0f, 0.0f, 20.0f), 0.5f);
			}
			else
			{
				AudioManager.Instance.Play("ButtonClick");
				AudioManager.Instance.Play("HintUse");
				hintCount = Game.Instance.playData.hint;
			}
		});
        levelComplete.gameObject.SetActive (false);
		gameObject.SetActive (false);
	}
}
