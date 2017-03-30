using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartLevel : MonoBehaviour {
	Button button;
	Text text;
	int level;

	public void Init(int level, string title)
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			UILevelPanel.Instance.gameObject.SetActive(false);
			Game.Instance.gameObject.SetActive(true);
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.level = level;
		text.text = "Level:" + level + "\n" + title; 
	}
}
