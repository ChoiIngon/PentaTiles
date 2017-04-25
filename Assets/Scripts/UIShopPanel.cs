using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPanel : MonoBehaviour {
	public Text hintCountText;
	public int hintCount {
		set {
			hintCountText.text = value.ToString ();
		}
	}
}
