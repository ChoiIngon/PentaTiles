using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour {

	//#if !UNITY_ADS // If the Ads service is not enabled...
	public string gameId; // Set this value from the inspector.
	public bool enableTestMode = true;
	//#endif

	// Use this for initialization
	IEnumerator Start () {
		Debug.Log ("initialize unity ads");
		if (false == Advertisement.isSupported) { // If runtime platform is supported...
			Debug.Log("advertisement is not supported");
			yield break;
			//Advertisement.Initialize(gameId, enableTestMode); // ...initialize.
		}

		Advertisement.Initialize (gameId); //, enableTestMode); // ...initialize.

		while (!Advertisement.isInitialized || !Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.5f);
		}

		Debug.Log ("unity ads is ready");
	}

	public void Show()
	{
		Debug.Log ("show ad");
		if (Advertisement.IsReady())
		{
			Advertisement.Show();
		}
	}

}
