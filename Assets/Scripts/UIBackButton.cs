using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackButton : MonoBehaviour {
	public Vector3 direction;
	Button button;
	// Use this for initialization
	void Start () {
		button = GetComponent<Button> ();
		button.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.rootPanel.ScrollScreen(direction);
		});
	}
}
