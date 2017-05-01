using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement : Quest.Data { 
	public class Type {
		public const string QuestCompleteCount = "QuestCompleteCount";
		public const string StarCollectCount = "StarCollectCount";
		public const string StageCompleteCount = "StageCompleteCount";
		public const string AdsWatchCount = "AdsWatchCount";
		public const string AdsRemove = "AdsRemove";
		public const string WorldOpen = "WorldOpen";
		public const string BlockOpen = "BlockOpen";
		public const string HintUse = "HintUse";
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

	public void SetProgress(Quest.Progress progress)
	{
		
	}

	public override string ToString ()
	{
		return "Achievement:{id:" + id + ", name:" + name + ", description:" + description + ", progress/goal:" + GetProgress().progress + "/" + GetProgress().goal +"}";
	}

	public static int Compare(Achievement lhs, Achievement rhs)
	{
		if (null == lhs && null == rhs) {
			return 0;
		}

		if (null == lhs) { // rhs is greater
			return -1;
		}

		if (null == rhs) {
			return 0;
		}

		if (Quest.State.Started == lhs.state) {
			if (Quest.State.Started == rhs.state) {
				float lhsProgress = 0 == lhs.GetProgress().progress ? 0.0f : (float)lhs.GetProgress ().progress / (float)lhs.GetProgress ().goal;
				float rhsProgress = 0 == rhs.GetProgress().progress ? 0.0f : (float)rhs.GetProgress ().progress / (float)rhs.GetProgress ().goal;

				if (lhsProgress > rhsProgress) {
					return -1;
				} else if (lhsProgress < rhsProgress) {
					return 1;
				} else {
					return lhs.id.CompareTo (rhs.id);
				}
			} else if (Quest.State.Complete == rhs.state) {
				return 1;
			} else if (Quest.State.Rewared == rhs.state) {
				return -1;
			}
		}
		else if (Quest.State.Complete == lhs.state) {
			if (Quest.State.Started == rhs.state) {
				return -1;
			} else if (Quest.State.Complete == rhs.state) {
				return lhs.id.CompareTo (rhs.id);
			} else if (Quest.State.Rewared == rhs.state) {
				return -1;
			}
		}
		else if (Quest.State.Rewared == lhs.state) {
			if (Quest.State.Started == rhs.state) {
				return 1;
			} else if (Quest.State.Complete == rhs.state) {
				return 1;
			} else if (Quest.State.Rewared == rhs.state) {
				return lhs.id.CompareTo (rhs.id);
			}
		}

		return lhs.id.CompareTo (rhs.id);
	}

}
