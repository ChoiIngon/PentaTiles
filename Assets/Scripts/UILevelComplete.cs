using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelComplete : MonoBehaviour {
	public Button redo;
	public Button next;

    public Image[] stars;
	public GameObject result;
	// Use this for initialization
	void Start () {
		redo.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
			Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel);
		});
		next.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);

			Config.StageInfo stageInfo = Game.Instance.config.FindStageInfo(Game.Instance.playData.currentStage);
			if(Game.Instance.playData.currentLevel < stageInfo.totalLevel)
			{
				Game.Instance.playData.currentLevel++;
				Game.Instance.StartLevel(Game.Instance.playData.currentStage, Game.Instance.playData.currentLevel);
			}
			else
			{
				Map.Instance.gameObject.SetActive(false);
				Game.Instance.playData.currentStage = 0;
				Game.Instance.playData.currentLevel = 0;
				Game.Instance.ScrollScreen(2.0f);
			}
		});
	}

	void OnEnable()
	{
        result.transform.localScale = Vector3.zero;
        for (int i = 0; i < stars.Length; i++)
        {
            Image star = stars[i];
            star.transform.localScale = Vector3.zero;
        }
    }

	public IEnumerator Activate() {
		gameObject.SetActive (true);
        
        iTween.ScaleTo(result, Vector2.one, 0.5f);
        yield return new WaitForSeconds(0.5f);

        const float time = 0.2f;
        for (int i = 0; i < stars.Length; i++)
        {
            Image star = stars[i];
            star.transform.localScale = Vector3.zero;
            iTween.ScaleTo(star.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "z", 1.0f, "time", time));
            yield return new WaitForSeconds(time);
        }

        while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
