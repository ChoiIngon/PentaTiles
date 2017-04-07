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

	public UIStageInfo.Info currentStage;
	public int currentLevel;
	public List<StageData> stageDatas;
	public int hint;

    public float bestCompleteTime = float.MaxValue;
}
