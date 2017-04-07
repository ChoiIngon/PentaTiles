using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {
	public Button redo;
	public Button next;

	public GameObject result;
	// Use this for initialization
	void Start () {
		redo.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
			Game.Instance.StartLevel(Game.Instance.playData.currentStage.stage, Game.Instance.playData.currentLevel);
		});
		next.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
			if(Game.Instance.playData.currentLevel < Game.Instance.playData.currentStage.total_level)
			{
				Game.Instance.playData.currentLevel++;
			}
			else
			{
				int currentStage = Game.Instance.playData.currentStage.stage;
				if(currentStage < Game.Instance.stageInfos.stage_infos.Length)
				{
					Game.Instance.playData.currentStage = Game.Instance.stageInfos.stage_infos[currentStage];
					Game.Instance.playData.currentLevel = 1;
				}
			}
			Game.Instance.StartLevel(Game.Instance.playData.currentStage.stage, Game.Instance.playData.currentLevel);
		});
	}

	void OnEnable()
	{
		result.transform.localScale = Vector3.zero;
		iTween.ScaleTo (result, Vector2.one, 0.5f);
	}

	void OnDisable()
	{
	}

	public IEnumerator Activate() {
		gameObject.SetActive (true);
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
