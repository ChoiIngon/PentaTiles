using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour {
	Button button;
	Text text;
	int stage;

	public void Init(int stage, string title)
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.stagePanel.gameObject.SetActive(false);
			Game.Instance.levelPanel.Init(stage);
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.stage = stage;
		text.text = "Stage:" + stage + "\n" + title; 
	}
}
