using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[XmlRoot("Config")]
public class Config {
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

    [XmlArray("Worlds")]
    [XmlArrayItem("World")]
    public List<WorldInfo> worldInfos = new List<WorldInfo>();

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
    }
    public List<StageInfo> stageInfos = new List<StageInfo>();

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

    [XmlArray("Achievements")]
    [XmlArrayItem("Achievement")]
    public List<AchievementInfo> achievementInfos = new List<AchievementInfo>();

    [XmlArray("Blocks")]
    [XmlArrayItem("Block")]
    public List<BlockInfo> blockInfos = new List<BlockInfo>();

    [XmlType("Block")]
    public class BlockInfo
    {
        [XmlAttribute("id")]
        public string id;
        [XmlAttribute("stage")]
        public int stage;
        [XmlAttribute("level")]
        public int level;
    }

    public StageInfo FindStageInfo(int id) {
		return stageInfos [id - 1];
	}

	public static Config Load() {
		TextAsset textAsset = (TextAsset)Resources.Load ("Config");
		MemoryStream stream = new MemoryStream (System.Text.Encoding.UTF8.GetBytes(textAsset.text));
		XmlSerializer serializer = new XmlSerializer (typeof(Config));
		Config config = serializer.Deserialize (stream) as Config;
		stream.Close ();

		int worldMaxID = 0;
		int stageMaxID = 0;
		foreach (WorldInfo worldInfo in config.worldInfos) {
            worldMaxID = Mathf.Max (worldMaxID, worldInfo.id);
			foreach (StageInfo stageInfo in worldInfo.stageInfos) {
				stageMaxID = Mathf.Max (stageMaxID, stageInfo.id);
			}
		}

		List<WorldInfo> worldInfos = new List<WorldInfo>(new WorldInfo[worldMaxID]);
		List<StageInfo> stageInfos = new List<StageInfo>(new StageInfo[stageMaxID]);

		foreach (WorldInfo worldInfo in config.worldInfos) {
			worldInfos [worldInfo.id - 1] = worldInfo;
			foreach (StageInfo stageInfo in worldInfo.stageInfos) {
#if UNITY_EDITOR
				Debug.Assert(null == stageInfos [stageInfo.id - 1], "duplicated stage id:" + stageInfo.id);
#endif
				stageInfos [stageInfo.id - 1] = stageInfo;
			}
		}

        config.worldInfos = worldInfos;
		config.stageInfos = stageInfos;
		return config;
	}
}
