﻿using System.Collections;
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
		yield return StartCoroutine(ActivateRewardAds());
	}

	private IEnumerator ActivateRewardAds()
	{
		Game.Instance.gamePanel.adsButton.gameObject.SetActive(false);
		while (!Advertisement.isInitialized || !Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.5f);
		}
		Game.Instance.gamePanel.adsButton.gameObject.SetActive(true);
		Debug.Log ("unity ads is ready");
	}
	public void Show()
	{
		if (true == Game.Instance.playData.adsFree) {
			return;
		}

		if (2 <= Game.Instance.playData.openWorlds.Length && false == Game.Instance.playData.openWorlds [1]) {
			return;
		}

		if (100 <= Game.Instance.playData.hint) {
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
		watchCount++;
		Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
			{"stage", Game.Instance.playData.currentStage},
			{"level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel},
			{"count",  watchCount} 
		});
		FirebaseAnalytics.LogEvent("AdsWatch", new Parameter[] {
			new Parameter("stage", Game.Instance.playData.currentStage),
			new Parameter(FirebaseAnalytics.ParameterLevel, Game.Instance.playData.currentStage + "-" + Game.Instance.playData.currentLevel),
			new Parameter(FirebaseAnalytics.ParameterQuantity, watchCount),
			new Parameter("show_result", result.ToString())
		});

		StartCoroutine(ActivateRewardAds());

		if (ShowResult.Failed == result) {
			return;
		}

		Game.Instance.AddHint (rewardHintCount);

		if (ShowResult.Finished == result) {
			Quest.Update (Achievement.Type.AdsWatchCount, "");
		}
	

    }
	public void ShowRewardAds()
	{
		lastAdShowTime = Time.realtimeSinceStartup;
		lastAdShowCount = 0;
		var options = new ShowOptions { resultCallback = OnAdsComplete };
		Advertisement.Show("rewardedVideo", options);
	}

}
