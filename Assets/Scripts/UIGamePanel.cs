using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour {
	public Text level;
	public Button backButton;
	public Button redoButton;
	public Button hintButton;
	public UILevelComplete levelComplete;
    public UIStageComplete stageComplete;
	// Use this for initialization
	void Start () {
		backButton.onClick.AddListener (() => {
			Map.Instance.gameObject.SetActive (false);
            levelComplete.gameObject.SetActive(false);
		});
		redoButton.onClick.AddListener (() => {
			Game.Instance.StartLevel(Game.Instance.playData.currentStage.stage, Game.Instance.playData.currentLevel);
			AudioManager.Instance.Play("ButtonClick");
		});
		hintButton.onClick.AddListener (() => {
			Map.Instance.UseHint();
			AudioManager.Instance.Play("ButtonClick");
		});
        levelComplete.gameObject.SetActive (false);
	}
}
