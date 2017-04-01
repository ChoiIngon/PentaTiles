using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStagePanel : MonoBehaviour {
	
	public UIStageInfo stageInfoPrefab;
	public List<UIStageInfo> stageInfos;
	public Transform content;
	// Use this for initialization
	public void Init () {
		stageInfos = new List<UIStageInfo> ();
		foreach(UIStageInfo.Info info in Game.Instance.stageInfos.stage_infos)
		{	
			UIStageInfo stageInfo = GameObject.Instantiate<UIStageInfo> (stageInfoPrefab);
			stageInfo.transform.SetParent (content, false);
			stageInfo.Init (info);
			stageInfos.Add (stageInfo);
		}

		Map.Instance.gameObject.SetActive (false);
	}

	public UIStageInfo GetStageInfo(int stage)
	{
		if (0 >= stage || stageInfos.Count < stage) {
			throw new System.Exception ("invalid stage id:" + stage);
		}
		return stageInfos [stage - 1];
	}
}
