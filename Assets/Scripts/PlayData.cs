using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayData {
	[System.Serializable]
	public class StageData {
		public int stage;
		public int level;
	}

	public UIStageInfo.Info current_stage;
	public int current_level;
	public List<StageData> stageDatas;
	public int hint;
}
