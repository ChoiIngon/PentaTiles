using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldOpenPanel : MonoBehaviour {
	public Image image;
	public Text text;
	public Sprite[] blockSprites;
	public Button okButton;

	void Start() {
		okButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
		});
	}

	public IEnumerator Open(int world)
	{
		if (0 >= world || blockSprites.Length < world) {
			yield break;
		}

		AudioManager.Instance.Play("LevelClear");
		gameObject.SetActive (true);

		Sprite sprite = blockSprites [world - 1];
		image.rectTransform.sizeDelta = sprite.rect.size;
		image.sprite = sprite;

		iTween.ScaleFrom(image.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.0f, "time", 0.2f));
		iTween.ScaleFrom(text.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.2f, "time", 0.2f));
		iTween.ScaleFrom(okButton.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.4f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
