using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardPanel : MonoBehaviour {
	RectTransform rectTransform;
	public Image image;
	public GameObject hintCount;
	public Text hintCountText;
	public Button okButton;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
		rectTransform.anchoredPosition = Vector2.zero;
		gameObject.SetActive (false);
	}

	void Start() {
		okButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
		});
	}
		
	public IEnumerator Open()
	{
		AudioManager.Instance.Play("LevelClear");
		Map.Instance.enableTouchInput = false;
		gameObject.SetActive (true);
		hintCountText.text = Game.Instance.playData.hint.ToString ();
		iTween.ScaleFrom(image.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.0f, "time", 0.2f));
		iTween.ScaleFrom(hintCount, iTween.Hash ("scale", Vector3.zero, "delay", 0.2f, "time", 0.2f));
		iTween.ScaleFrom(okButton.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.4f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
		Map.Instance.enableTouchInput = true;
	}
}
