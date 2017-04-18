using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
		Achievement achievement = new Achievement (
			"QUEST_AD_KILLER", 
			"AD Killer", 
			"Watch video advertisement until the end 100 times", 
			new Quest.Progress ("", Achievement.Type.AdsWatchCount, "", 100)
		);
        Quest.onProgress += (Quest.Progress progress) =>
        {
            Debug.Log("name:" + progress.name + ", " + progress.progress + "/" + progress.goal);
        };
        Quest.onComplete += (Quest.Data data) =>
        {
            Debug.Log("name:" + data.name + " complete");
        };
		Quest.datas.Add(achievement.id, achievement);
		for (int i = 0; i < 101; i++) {
			Quest.Update (Achievement.Type.AdsWatchCount, "");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
