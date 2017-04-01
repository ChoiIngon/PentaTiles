using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {
	public Button redo;
	public Button next;
	public bool isNext;

	// Use this for initialization
	void Start () {
		isNext = false;
		redo.onClick.AddListener (() => {
			isNext = false;
			gameObject.SetActive(false);
			Game.Instance.StartLevel(Game.Instance.playData.current_stage.stage, Game.Instance.playData.current_level);
		});
		next.onClick.AddListener (() => {
			isNext = true;
			gameObject.SetActive(false);
			if(Game.Instance.playData.current_level < Game.Instance.playData.current_stage.total_level)
			{
				Game.Instance.playData.current_level++;
			}
			else
			{
				int currentStage = Game.Instance.playData.current_stage.stage;
				if(currentStage < Game.Instance.stageInfos.stage_infos.Length)
				{
					Game.Instance.playData.current_stage = Game.Instance.stageInfos.stage_infos[currentStage];
					Game.Instance.playData.current_level = 1;
				}
			}
			Game.Instance.StartLevel(Game.Instance.playData.current_stage.stage, Game.Instance.playData.current_level);
		});
	}

	public IEnumerator Activate() {
		gameObject.SetActive (true);
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
