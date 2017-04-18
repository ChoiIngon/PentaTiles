using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement : Quest.Data {
	public class Type {
		public const string QuestCompleteCount = "QuestCompleteCount";
		public const string LevelCompleteCount = "LevelCompleteCount";
		public const string StageCompleteCount = "StageCompleteCount";
		public const string AdsWatchCount = "AdsWatchCount";
		public const string WorldOpen = "WorldOpen";
		public const string BlockOpen = "BlockOpen";
	}

	public string description;

	public Achievement(string id, string name, string description, Quest.Progress progress) : base()
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.progress.Add (progress);
		state = Quest.State.StartWait;
		Start ();
	}

	public Quest.Progress GetProgress()
	{
		return progress [0];
	}
}
