using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayData {
	[System.Serializable]
	public class StageData {
		public int id;
		public int clearLevel;
	}

    [System.Serializable]
    public class AchievementData
    {
        public string id;
        public int progress;
        public Quest.State state;
    }

	public int currentStage;
	public int currentLevel;

	public int hint;
	public int star;

	public bool[] openWorlds;		// world open state
	public StageData[] stageDatas;	// each stage progress state
    public AchievementData[] achievementDatas;
	public StageData GetCurrentStageData() {
		if (0 >= currentStage || stageDatas.Length < currentStage) {
			return null;
		}
		return stageDatas [currentStage - 1];
	}

	public void Save()
	{
		Debug.Log ("saved \'playdata.dat\' to " + Application.persistentDataPath + "/playdata.dat");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playdata.dat");
		bf.Serialize(file, this);
		file.Close();
	}

	public void Load()
	{
		Debug.Log ("loaded \'playdata.dat\' from " + Application.persistentDataPath + "/playdata.dat");
		openWorlds = new bool[Game.Instance.config.worldInfos.Count];
		stageDatas = new PlayData.StageData[Game.Instance.config.stageInfos.Count];

		PlayData tmpPlayData = null;
		if (File.Exists (Application.persistentDataPath + "/playdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playdata.dat", FileMode.Open);
			tmpPlayData = (PlayData)bf.Deserialize (file);
			file.Close ();
		}

		if (null != tmpPlayData) {
			hint = tmpPlayData.hint;
			star = tmpPlayData.star;
			for (int i = 0; i < tmpPlayData.openWorlds.Length; i++) {
				openWorlds [i] = false;
				if (i < openWorlds.Length) {
					openWorlds [i] = tmpPlayData.openWorlds [i];
				}
			}

			for (int i = 0; i < tmpPlayData.stageDatas.Length; i++) {
				if (i < stageDatas.Length) {
					stageDatas[i] = tmpPlayData.stageDatas[i];
				}
			}
		}

		foreach (Config.WorldInfo worldInfo in Game.Instance.config.worldInfos) {
			if (star >= worldInfo.openStar) {
				openWorlds [worldInfo.id - 1] = true;
			}
		}

		for (int i = 0; i < stageDatas.Length; i++) {
			if (null == stageDatas[i]) {
				PlayData.StageData stageData = new PlayData.StageData ();
				stageData.id = i + 1;
				stageData.clearLevel = 0;
				stageDatas [i] = stageData;
			}
		}
	}
}
