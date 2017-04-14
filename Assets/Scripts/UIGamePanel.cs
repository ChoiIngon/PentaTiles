using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour {
	public Text level;
	public Button backButton;
	public Button redoButton;
	public Button hintButton;
	public Text hintText;
	public UILevelComplete levelComplete;
    public UIStageComplete stageComplete;
	// Use this for initialization

	public int hintCount {
		set {
			if (99 < value) {
				hintText.text = "99+";
			} else {
				hintText.text = value.ToString ();
			}
		}
	}
	void Start () {
		backButton.onClick.AddListener (() => {
			Map.Instance.gameObject.SetActive (false);
            levelComplete.gameObject.SetActive(false);
		});
		redoButton.onClick.AddListener (() => {
			Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel);
			AudioManager.Instance.Play("ButtonClick");
		});
		hintCount = Game.Instance.playData.hint;
		hintButton.onClick.AddListener (() => {
			Map.Instance.UseHint();
			hintCount = Game.Instance.playData.hint;
			AudioManager.Instance.Play("ButtonClick");
		});
        levelComplete.gameObject.SetActive (false);
	}
}
