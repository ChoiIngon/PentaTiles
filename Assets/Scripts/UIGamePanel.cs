using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour {
	public Text level;
	public Button backButton;
	public Button redoButton;
	public Button hintButton;
	public UIResultPanel resultPanel;
	// Use this for initialization
	void Start () {
		backButton.onClick.AddListener (() => {
			Map.Instance.gameObject.SetActive (false);
			resultPanel.gameObject.SetActive(false);
		});
		redoButton.onClick.AddListener (() => {
			Game.Instance.StartLevel(Game.Instance.playData.current_stage.stage, Game.Instance.playData.current_level);
			AudioManager.Instance.Play("ButtonClick");
		});
		hintButton.onClick.AddListener (() => {
			Map.Instance.UseHint();
			AudioManager.Instance.Play("ButtonClick");
		});
		resultPanel.gameObject.SetActive (false);
	}
}
