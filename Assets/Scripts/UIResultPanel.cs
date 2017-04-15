using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {
	public Button redoButton;
	public Button nextButton;

	public Text title;
	public GameObject star;

	public Text time;
	public Text move;

	// Use this for initialization
	void Start () {
		redoButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
			Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel);
		});
		nextButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
			Config.StageInfo stageInfo = Game.Instance.config.FindStageInfo(Game.Instance.playData.currentStage);
			if(Game.Instance.playData.currentLevel < stageInfo.totalLevel)
			{
				Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel + 1);
			}
			else
			{
				Map.Instance.gameObject.SetActive(false);
				Game.Instance.playData.currentLevel = 0;
				Game.Instance.rootPanel.ScrollScreen(1.0f);
			}
		});
	}

	public IEnumerator Open() {
		gameObject.SetActive (true);

		Config.StageInfo stageInfo = Game.Instance.config.FindStageInfo(Game.Instance.playData.currentStage);
		if(Game.Instance.playData.currentLevel < stageInfo.totalLevel)
		{
			title.text = "Level Complete";
		}
		else
		{
			title.text = "Stage Complete";
		}

		time.text = string.Format("{0:00}:{1:00}",(Game.Instance.playTime/60)%60,Game.Instance.playTime%60);
		move.text = Game.Instance.moveCount.ToString();

		iTween.ScaleFrom (gameObject, Vector3.zero, 0.5f);
		iTween.ScaleFrom(star, iTween.Hash("scale", Vector3.zero, "delay", 0.5f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
