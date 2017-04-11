using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour {
	[System.Serializable]
	public class Info
	{
		public int stage;
		public string name;
		public string description;
		public int total_level;
        public int page;
        public int open_star;
	}
	Info info;

	Button button;
	Text title;
	Text starText;
	Image starImage;
	Image panelImage;
	public Sprite open;
	public Sprite close;
	public bool isOpen;

    public void Init(Info info)
    {
        this.info = info;

        title = transform.FindChild("Text").GetComponent<Text>();
        panelImage = GetComponent<Image>();
        starImage = transform.FindChild("Star").GetComponent<Image>();
        starText = transform.FindChild("Star/Text").GetComponent<Text>();
        button = GetComponent<Button>();

        PlayData.StageData stageData = Game.Instance.playData.stageDatas[info.stage - 1];
        SetOpenLevel(stageData.level);

        isOpen = false;
        if (Game.Instance.playData.star >= info.open_star)
        {
            isOpen = true;
        }

        panelImage.sprite = isOpen ? open : close;
        starImage.gameObject.SetActive(isOpen);
        
        if (true == isOpen)
        {
            title.text = "<size=40>" + info.name + "</size>\n" + "<size=30>" + info.description + "</size>";
            button.onClick.AddListener(() => {
                Game.Instance.levelPanel.Init(this.info);
                Game.Instance.playData.currentStage = info;
                Game.Instance.ScrollScreen(-1.0f);
                AudioManager.Instance.Play("ButtonClick");
            });
        }
        else
        {
            title.text = "<size=40>" + info.name + "</size>\n" + "<size=30>" + "need " + info.open_star + " star to unlock stage" + "</size>";
            button.onClick.RemoveAllListeners();
        }
	}

	public void SetOpenLevel(int level) {
		starText.text = level + "/" + info.total_level; 
	}
}
