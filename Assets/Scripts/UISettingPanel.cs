using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class UISettingPanel : MonoBehaviour {
	[System.Serializable]
	public class SaveData
	{
		public bool isBgmOn;
		public bool isSoundOn;
	}

	public ToggleButton bgmToggle;
	public ToggleButton soundToggle;
	public Button closeButton;
	public RectTransform rectTransform;
	private SaveData saveData;
	// Use this for initialization
	void Start () {
		Load ();
		rectTransform = GetComponent<RectTransform> ();
		bgmToggle.isOn = saveData.isBgmOn;
		bgmToggle.onValueChanged.AddListener(value => {
			AudioManager.Instance.Play("ButtonClick");
			saveData.isBgmOn = bgmToggle.isOn;
			Save();

		});
		soundToggle.isOn = saveData.isSoundOn;
		soundToggle.onValueChanged.AddListener(value =>	{
			AudioManager.Instance.Play("ButtonClick");
			saveData.isSoundOn = soundToggle.isOn;
			AudioManager.Instance.Activate(soundToggle.isOn);

			Save();		

		});
		closeButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			gameObject.SetActive(false);

		});
		gameObject.SetActive (false);
	}

	void OnEnable()
	{
		Game.Instance.Pause (true);
	}

	void OnDisable()
	{
		if (true == Application.isPlaying) {
			Game.Instance.Pause (false);
		}
	}
	public void Save()
	{
		Debug.Log ("saved \'setting.dat\' to " + Application.persistentDataPath + "/setting.dat");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/setting.dat");
		bf.Serialize(file, saveData);
		file.Close();
	}

	public void Load()
	{
		Debug.Log ("loaded \'setting.dat\' from " + Application.persistentDataPath + "/setting.dat");
		if (File.Exists (Application.persistentDataPath + "/setting.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/setting.dat", FileMode.Open);
			saveData = (SaveData)bf.Deserialize (file);
			file.Close ();
		} else {
			saveData = new SaveData ();
			saveData.isBgmOn = true;
			saveData.isSoundOn = true;
		}
		AudioManager.Instance.Activate (saveData.isSoundOn);
	}
}
