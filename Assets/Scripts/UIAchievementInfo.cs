using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementInfo : MonoBehaviour {
    public Text title;
    public Text description;

    public Text progressText;
	public Image progressImage;
	public GameObject progressBar;
	public GameObject giftBox;
    public GameObject hint;
	public Button button;
    Quest.State state;
    public void Init(Achievement data)
    {
		title.text = data.name;
        description.text = data.description;
        state = data.state;

        Quest.Progress progress = data.GetProgress();
        progressText.text = progress.progress.ToString() + "/" + progress.goal.ToString();
		progressImage.transform.localScale = new Vector3((float)progress.progress / (float)progress.goal, 1.0f, 1.0f);

		button.enabled = false;
		button.onClick.RemoveAllListeners ();
		progressBar.SetActive (false);
		giftBox.SetActive (false);
		hint.SetActive (false);

        if (Quest.State.Complete == state)
		{
			button.enabled = true;
			button.onClick.AddListener (() => {
				data.state = Quest.State.Rewared;
				Game.Instance.AddHint(1);
				Game.Instance.achievementPanel.Sort();
			});
			giftBox.SetActive (true);
        }
        else if (Quest.State.Rewared == state)
        {
			hint.SetActive (true);
        }
        else
        {
			progressBar.SetActive (true);
        }
     }
}
