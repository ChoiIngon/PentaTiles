using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitlePanel : MonoBehaviour {
	public Text [] titleTexts;
	public Button playButton;
	// Use this for initialization
	public IEnumerator Init () {
		RectTransform rectTransform = GetComponent<RectTransform> ();
		rectTransform.anchoredPosition = Vector2.zero;

		foreach (Text titleText in titleTexts) {
			titleText.transform.localScale = Vector3.zero;
		}
		playButton.transform.localScale = Vector3.zero;
		playButton.onClick.AddListener (() => {
			StartCoroutine(PlayGame());
		});
		foreach (Text titleText in titleTexts) {
			AudioManager.Instance.Play ("BlockSelect");
			iTween.ScaleTo (titleText.gameObject, Vector3.one, 0.2f);
			yield return new WaitForSeconds (0.2f);
		}
		AudioManager.Instance.Play ("BlockSelect");
		iTween.ScaleTo (playButton.gameObject, Vector3.one, 0.2f);
	}
	
	IEnumerator PlayGame()
	{
		AudioManager.Instance.Play ("ButtonClick");
		iTween.ScaleTo (gameObject, Vector3.zero, 0.2f);
		yield return new WaitForSeconds (0.2f);
		gameObject.SetActive(false);
		Game.Instance.rootPanel.gameObject.SetActive(true);
	}
}
