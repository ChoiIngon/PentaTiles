using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour {
	public Button redo;
	public Button next;
	public bool isNext;

	// Use this for initialization
	void Start () {
		isNext = false;
		redo.onClick.AddListener (() => {
			isNext = false;
			gameObject.SetActive(false);
		});
		next.onClick.AddListener (() => {
			isNext = true;
			gameObject.SetActive(false);
		});
	}

	public IEnumerator Activate() {
		gameObject.SetActive (true);
		while (true == gameObject.activeSelf) {
			yield return null;
		}
	}
}
