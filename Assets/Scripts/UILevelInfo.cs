using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelInfo : MonoBehaviour {
	Button button;
	Text text;
	int stage;
	int level;

	public void Init(int stage, int level)
	{
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.playData.current_level = level;
			Map.Instance.gameObject.SetActive(true);
			Map.Instance.Init(stage, level);
			Game.Instance.ScrollScreen(-1.0f);
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.stage = stage;
		this.level = level;
		text.text = level.ToString (); 
	}
}
