using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementInfo : MonoBehaviour {
    public Text title;
    public Text description;
    public Text progressText;
    public Image progressImage;
    public Sprite giftBox;
    public Sprite hint;
    Quest.State state;
    public void Init(Achievement data)
    {
        title.text = data.name;
        description.text = data.description;
        state = data.state;

        Quest.Progress progress = data.GetProgress();
        progressText.text = progress.progress.ToString() + "/" + progress.goal.ToString();
		progressImage.transform.localScale = new Vector3((float)progress.progress / (float)progress.goal, 1.0f, 1.0f);

        if (Quest.State.Complete == data.state)
        {
        }
        else if (Quest.State.Rewared == data.state)
        {
        }
        else
        {
        }
     }
}
