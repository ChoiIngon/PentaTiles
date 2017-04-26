using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardPanel : MonoBehaviour {
    public enum RewardType
    {
        Hint,
        AdsFree
    }

    public Sprite[] rewardIconSprites;
    public Image rewardIconImage;
    public Text rewardText;

	public GameObject hintCount;
	public Text hintCountText;
	public Button okButton;

    RectTransform rectTransform;
    void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
		rectTransform.anchoredPosition = Vector2.zero;
		gameObject.SetActive (false);
	}

	void Start() {
		okButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);
		});
	}
		
	public IEnumerator Open(RewardType rewardType, int count)
	{
		AudioManager.Instance.Play("LevelClear");
		Map.Instance.enableTouchInput = false;

		gameObject.SetActive (true);

        rewardIconImage.sprite = rewardIconSprites[(int)rewardType];

        switch (rewardType)
        {
            case RewardType.Hint:
                rewardText.text = "You got " + count.ToString() + " hint!!";
                hintCount.SetActive(true);
                hintCountText.text = Game.Instance.playData.hint.ToString();
                iTween.ScaleFrom(hintCount, iTween.Hash("scale", Vector3.zero, "delay", 0.2f, "time", 0.2f));
                break;
            case RewardType.AdsFree:
                rewardText.text = "No Ads any more!!";
                hintCount.SetActive(false);
                break;
        }
		
        iTween.ScaleFrom(rewardIconImage.gameObject, iTween.Hash("scale", Vector3.zero, "delay", 0.0f, "time", 0.2f));
        iTween.ScaleFrom(rewardText.gameObject, iTween.Hash("scale", Vector3.zero, "delay", 0.2f, "time", 0.2f));
		iTween.ScaleFrom(okButton.gameObject, iTween.Hash ("scale", Vector3.zero, "delay", 0.4f, "time", 0.2f));
		while (true == gameObject.activeSelf) {
			yield return null;
		}
		Map.Instance.enableTouchInput = true;
	}
}
