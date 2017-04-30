using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;

public class UIStageInfo : MonoBehaviour {
	private Config.StageInfo info;
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
                    AudioManager.Instance.Play("ButtonClick");
					Game.Instance.playData.currentStage = info.id;

					Game.Instance.levelPanel.gameObject.SetActive(true);
					Game.Instance.rootPanel.ScrollScreen(new Vector3(-1.0f, 0.0f, 0.0f), () => {
						Game.Instance.stagePanel.gameObject.SetActive(false);
					});
				});
			}
			else
			{
				panelImage.sprite = closeSprite;
				title.text = info.name;
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
		clearLevel = stageData.clearLevel;
		description.text = "clear previous stage";
		Config.WorldInfo worldInfo = Game.Instance.config.worldInfos [info.world - 1];
		if (info.id == worldInfo.stageInfos [0].id) {
			description.text = "collect " + worldInfo.openStar + " stars";
		}
		open = stageData.open;
	}

	public int clearLevel {
		set {
			starText.text = value + "/" + info.totalLevel; 
		}
	}
}
