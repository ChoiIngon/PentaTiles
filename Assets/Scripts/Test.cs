using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public class Quest_Example : Quest.Data
    {
        public string npc;
        public string script;
        public Quest_Example()
        {
            this.id = "QUEST_EXAMPLE";
            this.name = "Example Quest";
            this.state = Quest.Data.State.StartWait;
            this.triggers.Add(new Quest.Trigger()); // return true only
            this.progress.Add(new Quest.ProgressSelector(
                new Quest.Progress[] {
                    new Quest.Progress("no progress name", "LevelComplete", "1_1", 1),
                    new Quest.Progress("no progress name", "LevelComplete", "1_2", 1),
                    new Quest.Progress("no progress name", "LevelComplete", "1_3", 1)
                }
            ));
            Start();
        }
        /*
            1. call Quest.IsAvaiable();
                - check all Quest.Trigger.IsAvaliable() return true
                - if all are true, change state to State.StartWait
            2. call Quest.Start();
                - call all Progress.Start()
                - change state to State.Started;
        */
    }
    // Use this for initialization
    void Start () {
        Quest.onProgress += (Quest.Progress progress) =>
        {
            Debug.Log("name:" + progress.name + ", " + progress.progress + "/" + progress.goal);
        };
        Quest.onComplete += (Quest.Data data) =>
        {
            Debug.Log("name:" + data.name);
        };
        Quest.datas.Add("QUEST_EXAMPLE", new Quest_Example());
        Quest.Update("LevelComplete", "1_3");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
