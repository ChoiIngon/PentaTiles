using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlockOpenPanel : MonoBehaviour {
	public Image image;
	public Text text;
	public Sprite[] blockSprites;
	public Button okButton;

	private Dictionary<string, Sprite> _blockSprites;
	private RectTransform rectTransform;

	void Start() {
		okButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
		});
		_blockSprites = new Dictionary<string, Sprite> ();
		foreach (Sprite sprite in blockSprites) {
			_blockSprites.Add (sprite.name, sprite);
		}

		rectTransform = GetComponent<RectTransform> ();
	}

	public IEnumerator Open(string blockID)
	{
		Map.Instance.enableTouchInput = false;
		rectTransform.anchoredPosition = Vector2.zero;
		AudioManager.Instance.Play("LevelClear");
		gameObject.SetActive (true);

		Sprite sprite = _blockSprites [blockID];
		image.rectTransform.sizeDelta = sprite.rect.size;
		image.sprite = sprite;

		iTween.ScaleFrom(image.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.0f, "time", 0.2f));
		iTween.ScaleFrom(text.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.2f, "time", 0.2f));
		iTween.ScaleFrom(okButton.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.4f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
		Map.Instance.enableTouchInput = true;
	}
}
