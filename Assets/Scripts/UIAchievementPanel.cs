using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchievementPanel : MonoBehaviour {
    Dictionary<string, UIAchievementInfo> achievementInfos;

    public void Init()
    {
        achievementInfos = new Dictionary<string, UIAchievementInfo>();
    }
}
