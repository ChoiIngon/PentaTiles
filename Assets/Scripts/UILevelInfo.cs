using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelInfo : MonoBehaviour {
	public Sprite unlockedLevelSprite;

	Image image;
	Button button;
	Text text;
	int stage;
	int level;

	public void Init(int stage, int level)
	{
		image = GetComponent<Image> ();
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.playData.currentLevel = level;
			Game.Instance.StartLevel(this.stage, this.level);
			Game.Instance.ScrollScreen(-1.0f);
			AudioManager.Instance.Play("ButtonClick");
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.stage = stage;
		this.level = level;
		button.enabled = false;
		text.gameObject.SetActive (false);
	}

	public void Unlock()
	{
		image.sprite = unlockedLevelSprite;
		text.text = level.ToString (); 
		button.enabled = true;
		text.gameObject.SetActive (true);
	}
}
