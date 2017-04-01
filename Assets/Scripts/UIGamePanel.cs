using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour {
	public Text level;
	public Button backButton;
	public Button hintButton;
	public UIResultPanel resultPanel;
	// Use this for initialization
	void Start () {
		backButton.onClick.AddListener (() => {
			Map.Instance.gameObject.SetActive (false);
			resultPanel.gameObject.SetActive(false);
		});
		hintButton.onClick.AddListener (() => {
			Map.Instance.UseHint();
		});
		resultPanel.gameObject.SetActive (false);
	}
}
