using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelComplete : MonoBehaviour {
	public UIResultPanel resultPanel;
	public UIWorldOpenPanel worldOpenPanel;

	public IEnumerator Open(int world) {
		gameObject.SetActive (true);
		resultPanel.gameObject.SetActive (false);
		worldOpenPanel.gameObject.SetActive (false);
		yield return StartCoroutine (resultPanel.Open ());
		if (0 != world) {
			yield return StartCoroutine (worldOpenPanel.Open (world));
		}
		gameObject.SetActive (false);
	}
}
