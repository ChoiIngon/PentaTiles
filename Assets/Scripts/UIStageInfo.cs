using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;

public class UIStageInfo : MonoBehaviour {
	Config.StageInfo info;

	private Button button;
	public Text title;
	public Text description;
	public Image starImage;
	public Text starText;

	private Image panelImage;
	public Sprite openSprite;
	public Sprite closeSprite;

	public bool open {
		set {
			starImage.gameObject.SetActive(value);

			if (true == value)
			{
				panelImage.sprite = openSprite;
				title.text = info.name;
				description.text = info.description;
				button.onClick.AddListener(() => {
					Game.Instance.levelPanel.Init(this.info);
					Game.Instance.playData.currentStage = info.id;
					Game.Instance.rootPanel.ScrollScreen(-1.0f);
					AudioManager.Instance.Play("ButtonClick");
				});
			}
			else
			{
				panelImage.sprite = closeSprite;
				title.text = info.name;
				description.text = "need " + info.openStar + " star to unlock stage";
				button.onClick.RemoveAllListeners();
			}
		}
	}

	public void Init(Config.StageInfo info)
    {
        this.info = info;

		panelImage = GetComponent<Image>();
        button = GetComponent<Button>();

        PlayData.StageData stageData = Game.Instance.playData.stageDatas[info.id - 1];
		SetClearLevel (stageData.clearLevel);

		open = false;
		if (Game.Instance.playData.star >= info.openStar) {
			open = true;
		}
	}

	public void SetClearLevel(int level) {
		starText.text = level + "/" + info.totalLevel; 
	}
}
