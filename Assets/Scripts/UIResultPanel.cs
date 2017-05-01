using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {
	public Button redoButton;
	public Button nextButton;

	public Text title;
	public GameObject star;

	public Text messageText;

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
				Game.Instance.levelPanel.gameObject.SetActive(true);
				Game.Instance.rootPanel.ScrollScreen(new Vector3(1.0f, 0.0f, 0.0f), () => {
					Game.Instance.gamePanel.gameObject.SetActive(false);
				});
			}
		});
	}

	public IEnumerator Open() {
		transform.localScale = Vector3.zero;
		star.transform.localScale = Vector3.zero;

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

		string[] message = {
			"great!",
			"awesome!",
			"amazing!",
			"fantastic!",
			"brilliant!",
			"stunning!",
			"wonderful!",
			"mavelous!"
		};
		messageText.text = message [Random.Range (0, message.Length)];

		iTween.ScaleTo (gameObject, Vector3.one, 0.5f);
		iTween.ScaleTo(star, iTween.Hash("scale", Vector3.one, "delay", 0.5f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
