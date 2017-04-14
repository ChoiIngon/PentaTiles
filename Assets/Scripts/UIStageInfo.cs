using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;

public class UIStageInfo : MonoBehaviour {
	Config.StageInfo info;

	Button button;
	Text title;
	Text starText;
	Image starImage;
	Image panelImage;
	public Sprite open;
	public Sprite close;
	public bool isOpen;

	public void Init(Config.StageInfo info)
    {
        this.info = info;

		panelImage = GetComponent<Image>();
        title = transform.FindChild("Text").GetComponent<Text>();
        starImage = transform.FindChild("Star").GetComponent<Image>();
        starText = transform.FindChild("Star/Text").GetComponent<Text>();
        button = GetComponent<Button>();

        PlayData.StageData stageData = Game.Instance.playData.stageDatas[info.id - 1];
		SetClearLevel (stageData.clearLevel);

        isOpen = false;
        //if (Game.Instance.playData.star >= info.o)
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
                Game.Instance.playData.currentStage = info.id;
                Game.Instance.ScrollScreen(-1.0f);
                AudioManager.Instance.Play("ButtonClick");
            });
        }
        else
        {
            //title.text = "<size=40>" + info.name + "</size>\n" + "<size=30>" + "need " + info. + " star to unlock stage" + "</size>";
            button.onClick.RemoveAllListeners();
        }
	}

	public void SetClearLevel(int level) {
		starText.text = level + "/" + info.totalLevel; 
	}
}
