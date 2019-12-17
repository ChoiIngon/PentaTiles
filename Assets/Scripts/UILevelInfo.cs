using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelInfo : MonoBehaviour {
	public Sprite lockedLevelSprite;
	public Sprite unlockedLevelSprite;
	public Image starImage;

	Image buttonImage;
	Button startLevelButton;
	Text text;
	int stage;
	int level;

	public void Awake()
	{
		buttonImage = GetComponent<Image> ();
		startLevelButton = GetComponent<Button> ();
		startLevelButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.gamePanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(-1.0f, 0.0f, 0.0f), () => {
				Game.Instance.StartLevel(this.stage, this.level);
				Game.Instance.levelPanel.gameObject.SetActive(false);
			});
		});
		text = transform.Find ("Text").GetComponent<Text> ();
	}

	public void Init(int stage, int level)
	{
		this.stage = stage;
		this.level = level;
		startLevelButton.enabled = false;
		starImage.gameObject.SetActive (false);
		text.gameObject.SetActive (false);
		buttonImage.sprite = lockedLevelSprite;
		PlayData.StageData stageData = Game.Instance.playData.GetCurrentStageData ();
		if (level <= stageData.clearLevel + 1) {
			Unlock ();
		}

		if (level <= stageData.clearLevel) {
			Complete ();
		}
	}

	public void Unlock()
	{
		PlayData.StageData stageData = Game.Instance.playData.GetCurrentStageData();
		if (level <= stageData.clearLevel + 1) {
			buttonImage.sprite = unlockedLevelSprite;
			text.text = level.ToString (); 
			startLevelButton.enabled = true;
			text.gameObject.SetActive (true);
		}
	}

	public void Complete()
	{
		PlayData.StageData stageData = Game.Instance.playData.GetCurrentStageData();
		if (level <= stageData.clearLevel) {
			starImage.gameObject.SetActive (true);
		}
	}
}
