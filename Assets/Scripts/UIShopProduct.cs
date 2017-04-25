using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopProduct : MonoBehaviour {
	public string productID;
	public string productName;
	public float price;

	private Text productNameText;
	private Text productPriceText;
	private Button button;
	// Use this for initialization
	void Start () {
		productNameText = transform.FindChild ("Name").GetComponent<Text>();
		productPriceText = transform.FindChild ("Price/Text").GetComponent<Text> ();
		button = GetComponent<Button> ();

		productNameText.text = productName;
		productPriceText.text = string.Format("{0:F2}", price);
		button.onClick.AddListener(() => {
			Game.Instance.BuyInAppProduct(productID);
		});
	}
}
