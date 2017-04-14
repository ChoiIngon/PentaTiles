using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayData {
	[System.Serializable]
	public class StageData {
		public int id;
		public int clearLevel;
	}

	public int currentStage;
	public int currentLevel;

	public int hint;
	public int star;

	public bool[] openWorlds;		// world open state
	public StageData[] stageDatas;	// each stage progress state

	public StageData GetCurrentStageData() {
		if (0 >= currentStage || stageDatas.Length < currentStage) {
			return null;
		}
		return stageDatas [currentStage - 1];
	}
}
