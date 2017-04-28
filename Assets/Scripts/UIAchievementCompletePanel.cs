using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class UIAchievementCompletePanel : MonoBehaviour {
	public float moveTime;
	public float stayTime;
	public Vector2 deltaMove;
	public Text title;

	private List<string> titles;
	private RectTransform rectTransform;
	private Coroutine coroutine;

	void Start () {
		titles = new List<string> ();
		rectTransform = GetComponent<RectTransform> ();
		gameObject.SetActive (false);
	}

	// onComplete
	public void Open(Quest.Data achievement) {
		gameObject.SetActive (true);
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventUnlockAchievement, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterAchievementId, achievement.id)
        });
        Game.Instance.stagePanel.newAchievement.SetActive (true);
		titles.Add (achievement.name);
		if (null == coroutine) {
			coroutine = StartCoroutine (_Open ());
		}
	}

	public IEnumerator _Open()
	{
		
		while (0 < titles.Count) {
			title.text = "'" + titles [0] + "' complete";
			float interpolate = 0.0f;
			while (1.0f > interpolate) {
				interpolate += Time.deltaTime / moveTime;
				rectTransform.anchoredPosition = Vector2.Lerp (Vector2.zero, deltaMove, interpolate);
				yield return null;
			}
			rectTransform.anchoredPosition = deltaMove;

			yield return new WaitForSeconds (1.0f);
		
			interpolate = 0.0f;
			while (1.0f > interpolate) {
				interpolate += Time.deltaTime / moveTime;
				rectTransform.anchoredPosition = Vector2.Lerp (deltaMove, Vector2.zero, interpolate);
				yield return null;
			}
			rectTransform.anchoredPosition = Vector2.zero;
			titles.RemoveAt (0);
		}
		coroutine = null;
		gameObject.SetActive (false);
	}
}
