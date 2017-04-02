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
			Game.Instance.StartLevel(Game.Instance.playData.current_stage.stage, Game.Instance.playData.current_level);
		});
		next.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
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
