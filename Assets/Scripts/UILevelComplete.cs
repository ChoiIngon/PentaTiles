using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelComplete : MonoBehaviour {
	public UIResultPanel resultPanel;

	public IEnumerator Open() {
		gameObject.SetActive (true);
		yield return StartCoroutine (resultPanel.Open ());
		gameObject.SetActive (false);
	}
}
