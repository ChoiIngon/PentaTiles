using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelComplete : MonoBehaviour {
	public UIResultPanel resultPanel;

	public IEnumerator Open(int world) {
		gameObject.SetActive (true);
		resultPanel.gameObject.SetActive (false);
		yield return StartCoroutine (resultPanel.Open ());
		gameObject.SetActive (false);
	}
}
