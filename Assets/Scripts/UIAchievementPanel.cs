using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementPanel : MonoBehaviour {
    public UIAchievementInfo achievementInfoPrefab;
    public RectTransform outerWindow;
    public Button backButton;
    Dictionary<string, UIAchievementInfo> achievementInfos;
    private Vector2 targetPosition;
    
    private void Start()
    {
        achievementInfos = new Dictionary<string, UIAchievementInfo>();
        backButton.onClick.AddListener(() =>
        {
            Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, 1.0f, 0.0f));
        });

       // GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //targetPosition = outerWindow.anchoredPosition;
        //outerWindow.anchoredPosition = new Vector2(0.0f, 1280.0f);
        //gameObject.SetActive(false);
    }
    /*
    private void OnEnable()
    {
        outerWindow.anchoredPosition = new Vector2(0.0f, 1280.0f);
        SlideIn();
    }

    public void SlideIn()
    {
        Debug.Log("SlideIn");
        iTween.ValueTo(outerWindow.gameObject, iTween.Hash(
            "from", outerWindow.anchoredPosition,
            "to", targetPosition,
            "time", 0.5f,
            "onupdatetarget", this.gameObject,
            "onupdate", "MoveOuterWindow"
            ));
    }

    public void MoveOuterWindow(Vector2 position)
    {
        outerWindow.anchoredPosition = position;
        Debug.Log(outerWindow.anchoredPosition);
    }
    */
}
