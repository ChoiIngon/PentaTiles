using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

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
	public UITitlePanel			titlePanel;
	public UIRootPanel          rootPanel;
	public UIStageSelectPanel   stagePanel;
	public UILevelSelectPanel   levelPanel;
	public UIGamePanel          gamePanel;
	public UISettingPanel       settingPanel;
    public UIAchievementPanel   achievementPanel;
	public UIAchievementCompletePanel achievementCompletePanel;
	public UIBlockOpenPanel		blockOpenPanel;
	public UIRewardPanel		rewardPanel;
	public UIShopPanel 			shopPanel;
	public GameObject           background;

    public AudioSource bgm;

	public Config config;
	public PlayData playData;

	public UnityAds unityAds;
	public Advertisement advertisement;
	public InAppPurchaser inAppPurchaser;
	public float playTime;
	public int moveCount;
	public Text versionText;

	IEnumerator Start()
	{
		versionText.text = "Ver : " + Application.version;
		Quest.onComplete += achievementCompletePanel.Open;
		inAppPurchaser = GetComponent<InAppPurchaser> ();

		unityAds = GetComponent<UnityAds> ();
		unityAds.showIntervalCount = 3;
		unityAds.showIntervalTime = 120;
		unityAds.rewardHintCount = 1;

		advertisement = GetComponent<Advertisement>();

		Map.Instance.editMode = false;
		Map.Instance.gameObject.SetActive (false);

		config = Config.Load ();
		playData.Load ();

		rootPanel.rectTransform.anchoredPosition = Vector2.zero;
		stagePanel.Init ();
		levelPanel.Init ();
		achievementPanel.Init ();
        shopPanel.Init();
		rootPanel.gameObject.SetActive (false);

		yield return StartCoroutine (titlePanel.Init ());

		iTween.RotateBy(background, iTween.Hash("y", 1.0f, "speed", 7.0f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.loop));
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
			int rewardCount = 0;
			playTime = Time.realtimeSinceStartup - playTime;
			PlayData.StageData stageData = playData.GetCurrentStageData ();

			if (playData.currentLevel > stageData.clearLevel) {
				playData.star += 1;
				stageData.clearLevel = playData.currentLevel;

				stagePanel.totalStarCount = playData.star;
				stagePanel.GetStageInfo (stageData.id).clearLevel = stageData.clearLevel;
				levelPanel.GetLevelInfo (stageData.clearLevel).Complete ();

				Quest.Update (Achievement.Type.StarCollectCount, "");

				Config.StageInfo stageInfo = config.FindStageInfo(stageData.id);
				if (playData.currentLevel < stageInfo.totalLevel)
				{
					levelPanel.GetLevelInfo(stageData.clearLevel + 1).Unlock();
				}
				else if (playData.currentLevel == stageInfo.totalLevel)
				{
					rewardCount = 1;
					Quest.Update (Achievement.Type.StageCompleteCount, "");
					if (stageInfo.id < config.stageInfos.Count) {
						playData.GetStageData(stageData.id + 1).open = true;
						stagePanel.GetStageInfo(stageData.id+1).open = true;
					}
				}

				Analytics.CustomEvent("LevelComplete", new Dictionary<string, object> {
					{"stage", playData.currentStage},
					{"level", playData.currentStage + "-" + playData.currentLevel},
					{"star", playData.star}
				});
			}

			GetNewOpenWorld();
			playData.Save ();

			yield return new WaitForSeconds (1.0f);
			yield return StartCoroutine(gamePanel.levelComplete.Open ());
			if (0 < rewardCount) {
				AddHint (rewardCount);
			}
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

				Analytics.CustomEvent ("OpenWorld_" + (i + 1).ToString(), new Dictionary<string, object> {
					{ "stage", playData.currentStage },
					{ "level", playData.currentStage + "-" + playData.currentLevel },
					{ "star", playData.star }
				});
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
				Quest.Update (Achievement.Type.BlockOpen, blockInfo.id);
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
		Quest.Update (Achievement.Type.HintUseCount, "");
		Analytics.CustomEvent("HintUse", new Dictionary<string, object> {
			{"stage", playData.currentStage},
			{"level", playData.currentStage + "-" + playData.currentLevel}
		});
        return true;
	}

	public void AddHint(int count)
	{
		playData.hint += count;
		playData.Save ();
		gamePanel.hintCount = playData.hint;
		shopPanel.hintCount = playData.hint;
		StartCoroutine (rewardPanel.Open (UIRewardPanel.RewardType.Hint, count));
	}

	public void BuyInAppProduct(string id)
	{
		inAppPurchaser.BuyProductID (id);
	}
}
