using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackButton : MonoBehaviour {
	Button button;
	// Use this for initialization
	void Start () {
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			Game.Instance.rootPanel.ScrollScreen(new Vector3(1.0f, 0.0f, 0.0f));
			AudioManager.Instance.Play("ButtonClick");
		});
	}
}
