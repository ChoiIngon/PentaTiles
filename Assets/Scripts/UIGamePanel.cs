using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
			Map.Instance.gameObject.SetActive (false);
            levelComplete.gameObject.SetActive(false);
		});
		adsButton.onClick.AddListener (() => {
			Game.Instance.unityAds.ShowRewardAds();
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
	}
}
