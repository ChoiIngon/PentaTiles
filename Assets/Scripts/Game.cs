using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
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

	public UIRootPanel rootPanel;
	public UIStageSelectPanel stagePanel;
	public UILevelSelectPanel levelPanel;
	public UIGamePanel gamePanel;
	public UISettingPanel settingPanel;
	public GameObject background;

    public AudioSource bgm;

	public Config config;
	public PlayData playData;

	private UnityAds unityAds;

	public float playTime;
	public int moveCount;

	void Start()
	{
		unityAds = GetComponent<UnityAds> ();
		Map.Instance.editMode = false;
		Map.Instance.gameObject.SetActive (false);

		config = Config.Load ();
		playData.Load ();

		rootPanel.rectTransform.anchoredPosition = Vector2.zero;
		stagePanel.Init ();

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
				stageData.clearLevel = playData.currentLevel;
				playData.star += 1;
				stagePanel.totalStarCount = playData.star;

				Analytics.CustomEvent("LevelComplete", new Dictionary<string, object> {
					{"stage", playData.currentStage},
					{"level", playData.currentStage + "-" + playData.currentLevel},
					{"star", playData.star}
				});
			}

			int newOpenWorld = GetNewOpenWorld ();
			if (0 != newOpenWorld) {
				Analytics.CustomEvent ("OpenWorld_" + newOpenWorld.ToString (), new Dictionary<string, object> {
					{ "stage", playData.currentStage },
					{ "level", playData.currentStage + "-" + playData.currentLevel },
					{ "star", playData.star }
				});
			}
			Config.StageInfo stageInfo = config.FindStageInfo (playData.currentStage);
			stagePanel.GetStageInfo (stageData.id).SetClearLevel(stageData.clearLevel);
			if (stageData.clearLevel < stageInfo.totalLevel ) {
				levelPanel.GetLevelInfo (stageData.clearLevel+1).Unlock ();
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
			if (false == playData.openWorlds [i] && playData.star >= config.worldInfos [i].openStar) {
				playData.openWorlds [i] = true;
				Config.WorldInfo worldInfo = config.worldInfos [i];
				foreach (Config.StageInfo stageInfo in worldInfo.stageInfos) {
					stagePanel.GetStageInfo (stageInfo.id).open = true;
				}
				return i + 1;
			}
		}
		return 0;
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
		Analytics.CustomEvent("HintUse", new Dictionary<string, object> {
			{"stage", Game.Instance.playData.currentStage},
			{"level", Game.Instance.playData.currentStage + "-" + Game.Instance.playData.currentLevel}
		});
		return true;
	}

	//#if UNITY_EDITOR
	private void OnGUI()
	{
		string text = "";
		text += "Platform : " + Application.platform.ToString () + "\n";
		text += "Stage : " + Map.Instance.stage + ", Level : " + Map.Instance.level + "\n";
		text += "Mode : " + (true == Map.Instance.editMode ? "Edit" : "Game") + "\n";
		text += "Map Size :" + Map.Instance.width + " x " + Map.Instance.height + "\n";
		text += "Ad interval:" + unityAds.showInterval + ", elapsed time:" + (Time.realtimeSinceStartup - unityAds.lastAdShowTime) + "\n";
		GUI.Label (new Rect (0, 0, 400, 100), text);
	}
	//#endif
}
