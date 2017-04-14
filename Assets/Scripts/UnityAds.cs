using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

public class UnityAds : MonoBehaviour {
	public string iosGameID; // Set this value from the inspector.
	public string androidGameID;
	public float showInterval;
	public int rewardHintCount;
	private float lastAdShowTime;

	IEnumerator Start () {
        if (false == Advertisement.isSupported) { // If runtime platform is supported...
			Debug.Log("advertisement is not supported");
			yield break;
		}

        lastAdShowTime = Time.realtimeSinceStartup;
        string gameID = androidGameID;
		if (Application.platform == RuntimePlatform.Android) {
			gameID = androidGameID;
		} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			gameID = iosGameID;
		}
		Debug.Log ("initialize unity ads(game_id:" + gameID +")");
		Advertisement.Initialize (gameID); //, enableTestMode); // ...initialize.
		while (!Advertisement.isInitialized || !Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.5f);
		}
        
        Debug.Log ("unity ads is ready");
	}

	public void Show()
	{
		Debug.Log ("show ad(interval:" + showInterval + ", elapsed time:" + (Time.realtimeSinceStartup - lastAdShowTime) +")");
		if (Advertisement.IsReady() && Time.realtimeSinceStartup - lastAdShowTime >= showInterval)
		{
			lastAdShowTime = Time.realtimeSinceStartup;
			Advertisement.Show();
			Game.Instance.playData.hint += rewardHintCount;
			Game.Instance.gamePanel.hintCount = Game.Instance.playData.hint;
            Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
				{"level", Game.Instance.playData.currentStage + "_" +  Game.Instance.playData.currentLevel}
            });
        }
	}

}
