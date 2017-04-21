using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayData {
    [System.Serializable]
	public class StageData {
		public int id;      // stage id
		public int clearLevel;   // clear level
        public bool open;   // whether open or close
    }

	public int currentStage;
	public int currentLevel;

	public int hint;
	public int star;

	public bool[] openWorlds;		// world open state
	public StageData[] stageDatas;	// each stage progress state
    
	public Dictionary<string, Achievement> achievements;
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
		stageDatas = new StageData[Game.Instance.config.stageInfos.Count];
		Quest.Init ();
		achievements = new Dictionary<string, Achievement> ();
		foreach (Config.AchievementInfo achievementInfo in Game.Instance.config.achievementInfos) {
			Achievement achievement = new Achievement (
				achievementInfo.id, 
				achievementInfo.name, 
				achievementInfo.description, 
				new Quest.Progress("", achievementInfo.type, achievementInfo.key, achievementInfo.goal)
			);
			achievements.Add (achievement.id, achievement);
			Quest.AddQuest (achievement);
		}
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
            openWorlds = tmpPlayData.openWorlds;
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

			achievements = tmpPlayData.achievements;
			Quest.Init ();
			foreach (var itr in achievements) {
				Achievement achievement = itr.Value;
				achievement.Start ();
				Quest.AddQuest (achievement);
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
