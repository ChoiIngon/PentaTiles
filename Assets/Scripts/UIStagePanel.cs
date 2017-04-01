using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStagePanel : MonoBehaviour {
	[System.Serializable]
	public class StageInfos {
		public UIStageInfo.Info[] stage_infos;
	}
	public StageInfos stageInfos;
	public UIStageInfo stageInfoPrefab;
	public Transform content;
	// Use this for initialization
	void Start () {
		TextAsset json = Resources.Load<TextAsset> ("StageInfo");
		stageInfos = JsonUtility.FromJson<StageInfos> (json.text);

		foreach(UIStageInfo.Info info in stageInfos.stage_infos)
		{	
			UIStageInfo stageInfo = GameObject.Instantiate<UIStageInfo> (stageInfoPrefab);
			stageInfo.transform.SetParent (content, false);
			stageInfo.Init (info);
		}

		Map.Instance.gameObject.SetActive (false);
	}
}
