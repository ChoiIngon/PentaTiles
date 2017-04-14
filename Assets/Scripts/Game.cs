using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

	public RectTransform uiRoot;
	public UIStagePanel stagePanel;
	public UILevelPanel levelPanel;
	public UIGamePanel gamePanel;
	public UISettingPanel settingPanel;
    public AudioSource bgm;
	public float scrollTime;
	public CanvasScaler canvasScaler;
	public Config config;
	public PlayData playData;
    public float levelStartTime;

	private UnityAds unityAds;
	private float uiWidth;
	void Start()
	{
		config = Config.Load ();
		Map.Instance.editMode = false;
		unityAds = GetComponent<UnityAds> ();

		Load ();

		//iTween.ShakePosition (background, new Vector3 (3.0f, 3.0f, 3.0f), 60.0f);
		uiWidth = canvasScaler.referenceResolution.x;
		Map.Instance.editMode = false;
		Map.Instance.gameObject.SetActive (false);

		stagePanel.Init ();
	}

	Coroutine scrollScreenCoroutine;
	public void ScrollScreen(float direction)
	{
		if (null != scrollScreenCoroutine) {
			return;	
		}

		scrollScreenCoroutine = StartCoroutine (_ScrollScreen (direction));
	}
	private IEnumerator _ScrollScreen(float direction)
	{
		float moveDistance = Mathf.Abs(uiWidth * direction);
		Vector2 position = uiRoot.anchoredPosition;
		float originalPosition = position.x;
		while (0.0f < moveDistance) {
			float delta = uiWidth * Time.deltaTime / scrollTime;
			position.x += delta * direction;
			uiRoot.anchoredPosition = position;
			moveDistance -= delta;
			yield return null;
		}

		uiRoot.anchoredPosition = new Vector2 (originalPosition + uiWidth * direction, uiRoot.anchoredPosition.y);
	
		scrollScreenCoroutine = null;
	}

	public void StartLevel(int stage, int level)
	{
        levelStartTime = Time.realtimeSinceStartup;

        gamePanel.level.text = "Level - " + level;
		Map.Instance.gameObject.SetActive(true);
		Map.Instance.Init(stage, level);
	}

	public bool ChekcOpenWorld()
	{
		for (int i = 0; i < playData.openWorlds; i++) {
		}
		return false;
	}

	public IEnumerator CompleteLevel() {
		if (true == Map.Instance.CheckComplete ()) {
			AudioManager.Instance.Play("LevelClear");

			PlayData.StageData stageData = playData.GetCurrentStageData ();
			if (stageData.clearLevel < playData.currentLevel) {
				stageData.clearLevel = playData.currentLevel;
				playData.star += 1;
				stagePanel.totalStarCount = playData.star;
			}

			Config.StageInfo stageInfo = config.FindStageInfo (playData.currentStage);
			stagePanel.GetStageInfo (stageData.id).SetClearLevel(stageData.clearLevel);
			if (stageData.clearLevel < stageInfo.totalLevel ) {
				levelPanel.GetLevelInfo (stageData.clearLevel+1).Unlock ();
			}

			Save ();
			unityAds.Show ();

			yield return new WaitForSeconds (1.0f);
			yield return StartCoroutine(gamePanel.levelComplete.Activate ());
		}
	}


	public void Save()
	{
		Debug.Log ("saved \'playdata.dat\' to " + Application.persistentDataPath + "/playdata.dat");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playdata.dat");
		bf.Serialize(file, playData);
		file.Close();
    }

	public void Load()
	{
		Debug.Log ("loaded \'playdata.dat\' from " + Application.persistentDataPath + "/playdata.dat");
		playData.openWorlds = new bool[config.worldInfos.Count];
		playData.stageDatas = new PlayData.StageData[config.stageInfos.Count];

		PlayData tmpPlayData = null;
		if (File.Exists (Application.persistentDataPath + "/playdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playdata.dat", FileMode.Open);
			tmpPlayData = (PlayData)bf.Deserialize (file);
			file.Close ();
		}
			
		if (null != tmpPlayData) {
			playData.hint = tmpPlayData.hint;
			playData.star = tmpPlayData.star;
			for (int i = 0; i < tmpPlayData.openWorlds.Length; i++) {
				if (i < playData.openWorlds.Length) {
					playData.openWorlds [i] = tmpPlayData.openWorlds [i];
				}
			}
				
			for (int i = 0; i < tmpPlayData.stageDatas.Length; i++) {
				if (i < playData.stageDatas.Length) {
					playData.stageDatas[i] = tmpPlayData.stageDatas[i];
				}
			}
		}

		for (int i = 0; i < playData.openWorlds.Length; i++) {
			if(null == playData.openWorlds [i])
			{
				playData.openWorlds [i] = false;
			}
		}

		for (int i = 0; i < playData.stageDatas.Length; i++) {
			if (null == playData.stageDatas[i]) {
				PlayData.StageData stageData = new PlayData.StageData ();
				stageData.id = i + 1;
				stageData.clearLevel = 0;
				playData.stageDatas [i] = stageData;
			}
		}
	}

	public void Pause(bool flag)
	{
		BoxCollider[] children = Map.Instance.transform.GetComponentsInChildren<BoxCollider> ();
		foreach (BoxCollider child in children) {
			child.enabled = !flag;
		}
	}

	#if UNITY_EDITOR
	private void OnGUI()
	{
		string text = "";
		text += "Platform : " + Application.platform.ToString () + "\n";
		text += "Stage : " + Map.Instance.stage + ", Level : " + Map.Instance.level + "\n";
		text += "Mode : " + (true == Map.Instance.editMode ? "Edit" : "Game") + "\n";
		text += "Map Size :" + Map.Instance.width + " x " + Map.Instance.height + "\n";
		GUI.Label (new Rect (0, 0, 400, 100), text);
	}
	#endif
}
