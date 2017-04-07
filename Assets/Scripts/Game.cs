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

	[System.Serializable]
	public class StageInfos {
		public UIStageInfo.Info[] stage_infos;
	}
		
	public RectTransform uiRoot;
	public UIStagePanel stagePanel;
	public UILevelPanel levelPanel;
	public UIGamePanel gamePanel;
	public UISettingPanel settingPanel;
    public AudioSource bgm;
	public float scrollTime;
	public CanvasScaler canvasScaler;
	public StageInfos stageInfos;
	public PlayData playData;
    public float levelStartTime;

	private UnityAds unityAds;
	private float uiWidth;
	void Start()
	{
		Map.Instance.editMode = false;
		unityAds = GetComponent<UnityAds> ();

		TextAsset json = Resources.Load<TextAsset> ("StageInfo");
		stageInfos = JsonUtility.FromJson<StageInfos> (json.text);
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
		float moveDistance = uiWidth;
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

	public IEnumerator CompleteLevel() {
		if (true == Map.Instance.CheckComplete ()) {
			AudioManager.Instance.Play("LevelClear");
            float playTime = Time.realtimeSinceStartup - levelStartTime;
            playData.bestCompleteTime = Mathf.Min(playData.bestCompleteTime, playTime);
            PlayData.StageData stageData = playData.stageDatas [playData.currentStage.stage - 1];
			stageData.level = Mathf.Max (stageData.level, playData.currentLevel);
			stagePanel.GetStageInfo (stageData.stage).SetOpenLevel (stageData.level);
			if (stageData.level < playData.currentStage.total_level) {
				levelPanel.GetLevelInfo (stageData.level+1).Unlock ();
			}
			Save ();
			unityAds.Show ();

			yield return new WaitForSeconds (1.0f);
            //gamePanel.resultPanel.
			yield return StartCoroutine(gamePanel.resultPanel.Activate ());
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
		if(File.Exists(Application.persistentDataPath + "/playdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playdata.dat", FileMode.Open);
			playData = (PlayData)bf.Deserialize(file);
			file.Close();
		}

		if (playData.stageDatas.Count < stageInfos.stage_infos.Length) {
			for (int i = playData.stageDatas.Count; i < stageInfos.stage_infos.Length; i++) {
				PlayData.StageData stageData = new PlayData.StageData ();
				stageData.stage = stageInfos.stage_infos [i].stage;
				stageData.level = 0;
				playData.stageDatas.Add (stageData);
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
