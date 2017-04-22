using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelInfo : MonoBehaviour {
	public Sprite unlockedLevelSprite;
	public Image starImage;

	Image buttonImage;
	Button button;
	Text text;
	int stage;
	int level;

	public void Init(int stage, int level)
	{
		buttonImage = GetComponent<Image> ();
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.StartLevel(this.stage, this.level);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(-1.0f, 0.0f, 0.0f));
			AudioManager.Instance.Play("ButtonClick");
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.stage = stage;
		this.level = level;
		button.enabled = false;
		starImage.gameObject.SetActive (false);
		text.gameObject.SetActive (false);
	}

	public void Unlock()
	{
		buttonImage.sprite = unlockedLevelSprite;
		text.text = level.ToString (); 
		button.enabled = true;
		text.gameObject.SetActive (true);
		PlayData.StageData stageData = Game.Instance.playData.stageDatas [stage - 1];
		if (level <= stageData.clearLevel) {
			starImage.gameObject.SetActive (true);
		}
	}
}
