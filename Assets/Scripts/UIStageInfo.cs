using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour {
	[System.Serializable]
	public class Info
	{
		public int stage;
		public string name;
		public string description;
		public int total_level;
        public int page;
        public int open_star;
	}
	Info info;

	Button button;
	Text title;
	Text starText;
	Image starImage;
	Image panelImage;
	public Sprite open;
	public Sprite close;
	public bool isOpen;

	public void Init(Info info)
	{
		panelImage = GetComponent<Image> ();
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.levelPanel.Init(this.info);
			Game.Instance.playData.currentStage = info;
			Game.Instance.ScrollScreen(-1.0f);
			AudioManager.Instance.Play("ButtonClick");
		});
		title = transform.FindChild ("Text").GetComponent<Text> ();
		starImage = transform.FindChild ("Star").GetComponent<Image> ();
		starText = transform.FindChild ("Star/Text").GetComponent<Text> ();

		this.info = info;


		PlayData.StageData stageData = Game.Instance.playData.stageDatas [info.stage - 1];

		SetOpenLevel (stageData.level);

		starImage.gameObject.SetActive (false);
		panelImage.sprite = close;
		isOpen = false;
		title.text = "<size=40>" + info.name + "</size>\n" + 
			"<size=30>" + "need " + info.open_star + " star to unlock stage"  + "</size>";
		if (Game.Instance.playData.star >= info.open_star) {
			starImage.gameObject.SetActive (true);
			panelImage.sprite = open;
			isOpen = true;

			title.text = "<size=40>" + info.name + "</size>\n" + 
				"<size=30>" + info.description + "</size>";
		}
	}

	public void SetOpenLevel(int level) {
		starText.text = level + "/" + info.total_level; 
	}
}
