using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : MonoBehaviour {
	public Button backButton;
	public UIResultPanel resultPanel;
	// Use this for initialization
	void Start () {
		backButton.onClick.AddListener (() => {
			Map.Instance.gameObject.SetActive (false);
			resultPanel.gameObject.SetActive(false);
		});
		resultPanel.gameObject.SetActive (false);
	}
}
