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
        watchCount = 1;

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
			Debug.Log("The ad was successfully shown.");
			Game.Instance.playData.hint += rewardHintCount;
			Game.Instance.gamePanel.hintCount = Game.Instance.playData.hint;
			Game.Instance.playData.Save ();
			StartCoroutine (rewardPanel.Open ());
			Analytics.CustomEvent("AdsWatch", new Dictionary<string, object> {
				{"stage", Game.Instance.playData.currentStage},
				{"level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel},
				{"count", watchCount++ } 
			});
                FirebaseAnalytics.LogEvent("HintUse" + new Parameter[] {
                    new Parameter("stage", Game.Instance.playData.currentStage),
                    new Parameter("level", Game.Instance.playData.currentStage + "-" +  Game.Instance.playData.currentLevel),
                    new Parameter("count", watchCount++),
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
