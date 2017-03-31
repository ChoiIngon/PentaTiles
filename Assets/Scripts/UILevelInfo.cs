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
			Map.Instance.gameObject.SetActive(true);
			stage = 1;
			level = 1;
			MapSaveData mapSaveData = Resources.Load<MapSaveData> (stage + "_" + level);
			Map.Instance.Init(mapSaveData);
			Game.Instance.ScrollScreen(-1.0f);
		});
		text = transform.FindChild ("Text").GetComponent<Text> ();

		this.stage = stage;
		this.level = level;
		text.text = level.ToString (); 
	}
}
