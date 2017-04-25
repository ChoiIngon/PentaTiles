using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using Firebase.Analytics;

public class UnityAds : MonoBehaviour {
	public string iosGameID; // Set this value from the inspector.
	public string androidGameID;

    public float showIntervalTime;
    public int showIntervalCount;
	public int rewardHintCount;

    [ReadOnly]
	public float lastAdShowTime;
    [ReadOnly]
    public int lastAdShowCount;
    [ReadOnly]
    public int watchCount;
    public UIRewardPanel rewardPanel;
	
	IEnumerator Start () {
        if (false == Advertisement.isSupported) { // If runtime platform is supported...
			Debug.Log("advertisement is not supported");
			yield break;
		}

        lastAdShowTime = Time.realtimeSinceStartup;
        lastAdShowCount = 0;
        watchCount = 0;

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
		if (true == Game.Instance.playData.adsFree) {
			return;
		}

        if(false == Advertisement.IsReady())
        {
            return;
        }

        if (lastAdShowCount++ < showIntervalCount)
        {
            return;
        }

        if (Time.realtimeSinceStartup - lastAdShowTime < showIntervalTime)
        {
            return;
        }

		lastAdShowTime = Time.realtimeSinceStartup;
        lastAdShowCount = 0;
		var options = new ShowOptions { resultCallback = OnAdsComplete };
		Advertisement.Show("video", options);
	}
	private void OnAdsComplete(ShowResult result)
	{
		switch (result)	{
		case ShowResult.Finished:
		case ShowResult.Skipped:
			Debug.Log ("The ad was successfully shown.");
			watchCount++;
			Game.Instance.AddHint (rewardHintCount);
			Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
				{"stage", Game.Instance.playData.currentStage},
				{"level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel},
				{"count",  watchCount} 
			});
			FirebaseAnalytics.LogEvent("AdsWatch", new Parameter[] {
				new Parameter("stage", Game.Instance.playData.currentStage),
				new Parameter("level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel),
				new Parameter("count", watchCount),
				new Parameter("show_result", result.ToString())
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
	public void ShowRewardAds()
	{
		lastAdShowTime = Time.realtimeSinceStartup;
		lastAdShowCount = 0;
		var options = new ShowOptions { resultCallback = OnRewaredAdsComplete };
		Advertisement.Show("rewardedVideo", options);
	}
	private void OnRewaredAdsComplete(ShowResult result)
	{
		switch (result)	{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			watchCount++;
			Game.Instance.AddHint (rewardHintCount);
			Analytics.CustomEvent("RewardedAdsWatch", new Dictionary<string, object> {
				{"stage", Game.Instance.playData.currentStage},
				{"level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel},
				{"count", watchCount } 
			});
			FirebaseAnalytics.LogEvent("RewardedAdsWatch", new Parameter[] {
				new Parameter("stage", Game.Instance.playData.currentStage),
				new Parameter("level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel),
				new Parameter("count", watchCount),
				new Parameter("show_result", result.ToString())
			});
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;

		}
	}
}
