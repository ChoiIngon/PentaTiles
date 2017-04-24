using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using Firebase.Analytics;
using Firebase.RemoteConfig;

public class Game : MonoBehaviour {
	private static Game _instance;  
	public static Game Instance {  
		get {  
			if (!_instance) {  
				_instance = (Game)GameObject.FindObjectOfType(typeof(Game));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "Game";  
					_instance = container.AddComponent<Game>();  
				}  
			}

			return _instance;  
		}  
	}

	public UIRootPanel          rootPanel;
	public UIStageSelectPanel   stagePanel;
	public UILevelSelectPanel   levelPanel;
	public UIGamePanel          gamePanel;
	public UISettingPanel       settingPanel;
    public UIAchievementPanel   achievementPanel;
	public UIAchievementCompletePanel achievementCompletePanel;
	public UIBlockOpenPanel		blockOpenPanel;
	public UIRewardPanel		rewardPanel;
	public GameObject           background;

    public AudioSource bgm;

	public Config config;
	public PlayData playData;

	public UnityAds unityAds;

	public float playTime;
	public int moveCount;

	void Start()
	{
		Quest.onComplete += achievementCompletePanel.Open;
		unityAds = GetComponent<UnityAds> ();
		Dictionary<string, object> defaultRemoteConfig = new Dictionary<string, object>();
		defaultRemoteConfig.Add("ads_interval_count", 3);
		defaultRemoteConfig.Add("ads_interval_time", 120);
		defaultRemoteConfig.Add("ads_reward_hint_count", 1);
		FirebaseRemoteConfig.SetDefaults(defaultRemoteConfig);

		unityAds.showIntervalCount = (int)FirebaseRemoteConfig.GetValue ("ads_interval_count").LongValue;
		unityAds.showIntervalTime = (float)FirebaseRemoteConfig.GetValue ("ads_interval_time").DoubleValue;
		unityAds.rewardHintCount = (int)FirebaseRemoteConfig.GetValue ("ads_reward_hint_count").LongValue;

		FirebaseRemoteConfig.FetchAsync (new System.TimeSpan(0, 0, 30)).ContinueWith((antecedent) => {
			FirebaseRemoteConfig.ActivateFetched ();
			unityAds.showIntervalCount = (int)FirebaseRemoteConfig.GetValue ("ads_interval_count").LongValue;
			unityAds.showIntervalTime = (float)FirebaseRemoteConfig.GetValue ("ads_interval_time").DoubleValue;
			unityAds.rewardHintCount = (int)FirebaseRemoteConfig.GetValue ("ads_reward_hint_count").LongValue;
		});

		Map.Instance.editMode = false;
		Map.Instance.gameObject.SetActive (false);

		config = Config.Load ();
		playData.Load ();

		rootPanel.rectTransform.anchoredPosition = Vector2.zero;
		stagePanel.Init ();
		levelPanel.Init ();
		achievementPanel.Init ();
		iTween.RotateBy(background, iTween.Hash("y", 1.0f, "speed", 7.0f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.loop));

		FirebaseAnalytics.LogEvent (FirebaseAnalytics.EventAppOpen);
    }

	public void StartLevel(int stage, int level)
	{
		playData.currentStage = stage;
		playData.currentLevel = level;
		playTime = Time.realtimeSinceStartup;
		moveCount = 0;
		Map.Instance.gameObject.SetActive(true);
		Map.Instance.Init(stage, level);
		gamePanel.level.text = "Level - " + level;

		Analytics.CustomEvent("LevelPlay", new Dictionary<string, object> {
			{"stage", playData.currentStage},
			{"level", playData.currentStage + "-" + playData.currentLevel}
		});

        FirebaseAnalytics.LogEvent("LevelPlay", new Parameter[] {
            new Parameter("stage", playData.currentStage),
            new Parameter("level", playData.currentStage + "-" + playData.currentLevel)
        });

        string newOpenBlockID = GetNewOpenBlock();
        if("" != newOpenBlockID && false == playData.openBlocks.ContainsKey(newOpenBlockID)) {
            playData.openBlocks.Add(newOpenBlockID, newOpenBlockID);
			StartCoroutine (blockOpenPanel.Open (newOpenBlockID));
        }
    }

	public void CheckLevelComplete()
	{
		StartCoroutine (_CheckLevelComplete ());
	}

	public IEnumerator _CheckLevelComplete() {
		if (true == Map.Instance.CheckComplete ()) {
			AudioManager.Instance.Play("LevelClear");
			playTime = Time.realtimeSinceStartup - playTime;
			PlayData.StageData stageData = playData.GetCurrentStageData ();
			if (stageData.clearLevel < playData.currentLevel) {
				playData.star += 1;
				stageData.clearLevel = playData.currentLevel;

				levelPanel.GetLevelInfo (stageData.clearLevel).Complete ();
				stagePanel.totalStarCount = playData.star;
				stagePanel.GetStageInfo (stageData.id).SetClearLevel (stageData.clearLevel);
				Quest.Update ("LevelComplete", "");
			}

			Config.StageInfo stageInfo = config.FindStageInfo(playData.currentStage);
			if (stageData.clearLevel < stageInfo.totalLevel)
            {
                levelPanel.GetLevelInfo(stageData.clearLevel + 1).Unlock();
            }
			else if (stageData.clearLevel == stageInfo.totalLevel && stageData.id < playData.stageDatas.Length && false == playData.stageDatas[stageData.id].open)
			{
				playData.stageDatas[stageData.id].open = true;
				stagePanel.GetStageInfo(stageData.id+1).open = true;
			}

            Analytics.CustomEvent("LevelComplete", new Dictionary<string, object> {
				{"stage", playData.currentStage},
				{"level", playData.currentStage + "-" + playData.currentLevel},
				{"star", playData.star}
			});

            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp, new Parameter[] {
                new Parameter("stage", playData.currentStage),
                new Parameter("level", playData.currentStage + "-" + playData.currentLevel),
                new Parameter("star", playData.star)
            });

			int newOpenWorld = GetNewOpenWorld();
			if (0 != newOpenWorld) {
				Analytics.CustomEvent ("OpenWorld_" + newOpenWorld.ToString (), new Dictionary<string, object> {
					{ "stage", playData.currentStage },
					{ "level", playData.currentStage + "-" + playData.currentLevel },
					{ "star", playData.star }
				});
                FirebaseAnalytics.LogEvent("OpenWorld_" + newOpenWorld.ToString(), new Parameter[] {
                    new Parameter("stage", playData.currentStage),
                    new Parameter("level", playData.currentStage + "-" + playData.currentLevel),
                    new Parameter("star", playData.star)
                });
            }

            playData.Save ();

			yield return new WaitForSeconds (1.0f);
			yield return StartCoroutine(gamePanel.levelComplete.Open (newOpenWorld));
			unityAds.Show ();
		}
	}
	private int GetNewOpenWorld()
	{
		for (int i = 0; i < playData.openWorlds.Length; i++) {
            
            Config.WorldInfo worldInfo = config.worldInfos[i];

            if (false == playData.openWorlds [i] && playData.star >= worldInfo.openStar) {
				playData.openWorlds [i] = true;
                stagePanel.GetStageInfo(worldInfo.stageInfos[0].id).open = true;
				return i + 1;
			}
		}
		return 0;
	}

    private string GetNewOpenBlock()
    {
        for (int i = 0; i < config.blockInfos.Count; i++)
        {
            Config.BlockInfo blockInfo = config.blockInfos[i];
            if (playData.currentStage == blockInfo.stage && playData.currentLevel == blockInfo.level)
            {
                return blockInfo.id;
            }
        }
        return "";
    }
	public bool UseHint()
	{
		if (0 >= playData.hint) {
			return false;
		}
			
		if (false == Map.Instance.UseHint ()) {
			return false;
		}
			
		playData.hint -= 1;
		playData.Save ();
		gamePanel.hintCount = playData.hint;
		Analytics.CustomEvent("HintUse", new Dictionary<string, object> {
			{"stage", playData.currentStage},
			{"level", playData.currentStage + "-" + playData.currentLevel}
		});

        FirebaseAnalytics.LogEvent("HintUse", new Parameter[] {
            new Parameter("stage", playData.currentStage),
            new Parameter("level", playData.currentStage + "-" + playData.currentLevel)
        });

        return true;
	}

	public void AddHint(int count)
	{
		playData.hint += count;
		playData.Save ();
		gamePanel.hintCount = playData.hint;
		StartCoroutine (rewardPanel.Open ());
	}

	//#if UNITY_EDITOR
	private void OnGUI()
	{
		string text = "";
		text += "Platform : " + Application.platform.ToString () + "\n";
		text += "Stage : " + Map.Instance.stage + ", Level : " + Map.Instance.level + "\n";
		text += "Mode : " + (true == Map.Instance.editMode ? "Edit" : "Game") + "\n";
		text += "Map Size :" + Map.Instance.width + " x " + Map.Instance.height + "\n";
		text += "Ads : " + 
			(int)(Time.realtimeSinceStartup - unityAds.lastAdShowTime) + "/" + unityAds.showIntervalTime + " sec, " +
			unityAds.lastAdShowCount + "/" + unityAds.showIntervalCount + " count, " +
			unityAds.rewardHintCount + " hint\n";
        GUI.Label (new Rect (0, 0, 500, 200), text);
	}
	//#endif
}
