using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementInfo : MonoBehaviour {
    public Text title;
    public Text description;
    public Text progressText;
    public Image progressImage;
    
    public void Init(Achievement data)
    {
        title.text = data.name;
        description.text = data.description;
    }

    public void SetProgress(Quest.Progress progress)
    {
        progressText.text = progress.progress.ToString() + "/" + progress.goal.ToString();
        progressImage.transform.localScale = new Vector3(progress.progress / progress.goal, 1.0f, 1.0f);
    }
}
