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

	public float scrollTime;
	public CanvasScaler canvasScaler;
	public StageInfos stageInfos;
	public PlayData playData;
	//public GameObject background;

	private float uiWidth;
	void Start()
	{
		TextAsset json = Resources.Load<TextAsset> ("StageInfo");
		stageInfos = JsonUtility.FromJson<StageInfos> (json.text);
		Load ();

		//iTween.ShakePosition (background, new Vector3 (3.0f, 3.0f, 3.0f), 60.0f);
		uiWidth = canvasScaler.referenceResolution.x;
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
		gamePanel.level.text = "Level - " + level;
		Map.Instance.gameObject.SetActive(true);
		Map.Instance.Init(stage, level);
	}

	public IEnumerator CompleteLevel() {
		if (true == Map.Instance.CheckComplete ()) {
			PlayData.StageData stageData = playData.stageDatas [playData.current_stage.stage - 1];
			stageData.level = Mathf.Max (stageData.level, playData.current_level);
			stagePanel.GetStageInfo (stageData.stage).SetOpenLevel (stageData.level);
			if (stageData.level < playData.current_stage.total_level) {
				levelPanel.GetLevelInfo (stageData.level+1).Unlock ();
			}
			Save ();
			AudioManager.Instance.Play("LevelClear");
			yield return StartCoroutine(gamePanel.resultPanel.Activate ());
		}
		yield break;
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
}
