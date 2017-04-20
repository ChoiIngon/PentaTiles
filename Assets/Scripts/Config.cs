using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[XmlRoot("Config")]
public class Config {		
	[XmlElement("World")]
	public List<WorldInfo> worldInfos = new List<WorldInfo>();

	[XmlType("World")]
	public class WorldInfo
	{
		[XmlAttribute("id")]
		public int id;
		[XmlAttribute("open_star")]
		public int openStar;
		[XmlElement("Stage")]
		public List<StageInfo> stageInfos = new List<StageInfo>();
	}

	[System.Serializable]
	public class StageInfo
	{
		[XmlAttribute("id")]
		public int id;
		[XmlAttribute("name")]
		public string name;
		[XmlAttribute("description")]
		public string description;
		[XmlAttribute("total_level")]
		public int totalLevel;
		public int openStar;
	}

    [XmlArray("Achievements")]
    [XmlArrayItem("Achievement")]
    public List<AchievementInfo> achievementInfos = new List<AchievementInfo>();

    [XmlType("Achievement")]
    public class AchievementInfo
    {
        [XmlAttribute("id")]
        public string id;
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("description")]
        public string description;
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("key")]
        public string key;
        [XmlAttribute("goal")]
        public int goal;
    }

    public List<StageInfo> stageInfos;
	public StageInfo FindStageInfo(int id) {
		return stageInfos [id - 1];
	}

	public static Config Load() {
		TextAsset textAsset = (TextAsset)Resources.Load ("Config");
		MemoryStream stream = new MemoryStream (System.Text.Encoding.UTF8.GetBytes(textAsset.text));
		XmlSerializer serializer = new XmlSerializer (typeof(Config));
		Config config = serializer.Deserialize (stream) as Config;
		stream.Close ();

		int worldCount = 0;
		int stageCount = 0;
		foreach (WorldInfo worldInfo in config.worldInfos) {
			worldCount = Mathf.Max (worldCount, worldInfo.id);
			foreach (StageInfo stageInfo in worldInfo.stageInfos) {
				stageCount = Mathf.Max (stageCount, stageInfo.id);
			}
		}

		List<WorldInfo> worldInfos = new List<WorldInfo>();
		worldInfos.AddRange (new WorldInfo[worldCount]);
		List<StageInfo> stageInfos = new List<StageInfo>();
		stageInfos.AddRange (new StageInfo[stageCount]);

		foreach (WorldInfo worldInfo in config.worldInfos) {
			worldInfos [worldInfo.id - 1] = worldInfo;
			foreach (StageInfo stageInfo in worldInfo.stageInfos) {
#if UNITY_EDITOR
				Debug.Assert(0 < stageInfo.id && stageCount >= stageInfo.id, "invalid stage id:" + stageInfo.id);
				Debug.Assert(null == stageInfos [stageInfo.id - 1], "duplicated stage id:" + stageInfo.id);
#endif
				stageInfo.openStar = worldInfo.openStar;
				stageInfos [stageInfo.id - 1] = stageInfo;
			}
		}

		config.worldInfos = worldInfos;
		config.stageInfos = stageInfos;
		return config;
	}
}
