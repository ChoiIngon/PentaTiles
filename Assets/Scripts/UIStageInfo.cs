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
	Button button;
	Text title;
	Text star;
	Info info;

	public void Init(Info info)
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.levelPanel.Init(this.info);
			Game.Instance.playData.currentStage = info;
			Game.Instance.ScrollScreen(-1.0f);
			AudioManager.Instance.Play("ButtonClick");
		});
		title = transform.FindChild ("Text").GetComponent<Text> ();
		star = transform.FindChild ("Star/Text").GetComponent<Text> ();

		this.info = info;
		title.text = "<size=40>" + info.name + "</size>\n" + 
			"<size=30>" + info.description + "</size>";

		PlayData.StageData stageData = Game.Instance.playData.stageDatas [info.stage - 1];

		SetOpenLevel (stageData.level);
	}

	public void SetOpenLevel(int level) {
		star.text = level + "/" + info.total_level; 
	}
}
