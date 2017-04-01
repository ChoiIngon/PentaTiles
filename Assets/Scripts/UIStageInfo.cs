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
	}
	Button button;
	Text title;
	Text star;
	Info info;

	public void Init(Info info)
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.levelPanel.Init(info);
			Game.Instance.playData.current_stage = info;
			Game.Instance.ScrollScreen(-1.0f);
		});
		title = transform.FindChild ("Text").GetComponent<Text> ();
		star = transform.FindChild ("Star/Text").GetComponent<Text> ();

		this.info = info;
		title.text = "<size=50>" + info.name + "</size>\n" + 
			"<size=40>" + info.description + "</size>";
		star.text = "0" + "/" + info.total_level; 
	}
}
