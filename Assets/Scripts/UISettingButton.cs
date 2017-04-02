using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => {
			Game.Instance.settingPanel.rectTransform.anchoredPosition = Vector2.zero;
			Vector3 position = Game.Instance.settingPanel.transform.position;
			position.x = 0.0f;
			Game.Instance.settingPanel.transform.position = position;
			Game.Instance.settingPanel.gameObject.SetActive(true);
			AudioManager.Instance.Play("ButtonClick");
		});
	}
}
