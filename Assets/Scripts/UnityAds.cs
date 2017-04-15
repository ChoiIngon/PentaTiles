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
	public float lastAdShowTime;
	public UIRewardPanel rewardPanel;
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
		if (Advertisement.IsReady() && Time.realtimeSinceStartup - lastAdShowTime >= showInterval)
		{
			lastAdShowTime = Time.realtimeSinceStartup;
			var options = new ShowOptions { resultCallback = OnAdsComplete };
			Advertisement.Show("video", options);
        }
	}

	private void OnAdsComplete(ShowResult result)
	{
		switch (result)	{
		case ShowResult.Finished:
		case ShowResult.Skipped:
			Debug.Log("The ad was successfully shown.");
			Game.Instance.playData.hint += rewardHintCount;
			Game.Instance.gamePanel.hintCount = Game.Instance.playData.hint;
			Game.Instance.playData.Save ();
			StartCoroutine (rewardPanel.Open ());
			Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
				{"level", Game.Instance.playData.currentStage + "_" +  Game.Instance.playData.currentLevel}
			});
			break;
		//case ShowResult.Skipped:
			//Debug.Log("The ad was skipped before reaching the end.");
			//break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;

		}
	}
}
