using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	[System.Serializable]
	public class AudioInfo {
		public string name;
		public AudioClip clip;
		public bool loop;
	}
	private static AudioManager _instance;  
	public static AudioManager Instance {  
		get {  
			if (!_instance) {  
				_instance = (AudioManager)GameObject.FindObjectOfType(typeof(AudioManager));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "AudioManager";  
					_instance = container.AddComponent<AudioManager>();  
				}  
				_instance.Init ();
			}

			return _instance;  
		}  
	}

	public AudioInfo[] audioInfos;
	private Dictionary<string, AudioSource> audioSources;

	void Init () {
		audioSources = new Dictionary<string, AudioSource> ();
		if (null == audioInfos) {
			return;
		}
		foreach(AudioInfo audioInfo in audioInfos)
		{
			GameObject audio = new GameObject ();
			audio.name = audioInfo.name;
			audio.transform.SetParent (transform);

			AudioSource audioSource = audio.AddComponent<AudioSource> ();
			audioSource.clip = audioInfo.clip;
			audioSource.loop = audioInfo.loop;
			audioSource.playOnAwake = false;
			audioSources.Add (audioInfo.name, audioSource);
		}
	}

	public void Play(string name) {
		if (true == Map.Instance.editMode) {
			return;
		}

		if (false == audioSources.ContainsKey (name)) {
			throw new System.Exception ("invalid audiosource name(" + name + ")");
		}

		AudioSource audioSource = audioSources [name];
		audioSource.Play ();
	}

	public void Mute(bool flag)
	{
		foreach (var itr in audioSources) {
			AudioSource audioSource = itr.Value;
			audioSource.mute = flag;
		}	
	}
}
