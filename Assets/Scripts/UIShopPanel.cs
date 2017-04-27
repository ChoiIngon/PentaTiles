using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPanel : MonoBehaviour {
	public Text hintCountText;
    public Transform content;
	public Button backButton;
    public int hintCount {
		set {
			hintCountText.text = value.ToString ();
		}
	}

    
    private Dictionary<string, UIShopProduct> shopProducts;
    public void Init()
    {
        shopProducts = new Dictionary<string, UIShopProduct>();
        for (int i = 0; i < content.childCount; i++)
        {
            UIShopProduct shopProduct = content.GetChild(i).GetComponent<UIShopProduct>();
            if(null == shopProduct)
            {
                continue;
            }

            shopProducts.Add(shopProduct.productID, shopProduct);
        }
		hintCount = Game.Instance.playData.hint;
		if (true == Game.Instance.playData.adsFree)
		{
			RemoveProduct(InAppPurchaser.Pentatiles.RemoveAds);
		}
		backButton.onClick.AddListener (() => {
			AudioManager.Instance.Play("ButtonClick");
			Game.Instance.stagePanel.gameObject.SetActive(true);
			Game.Instance.rootPanel.ScrollScreen(new Vector3(0.0f, -1.0f, 0.0f), () => {
				gameObject.SetActive(false);
			});
		});
		gameObject.SetActive (false);
    }

    public void RemoveProduct(string id)
    {
        if (false == shopProducts.ContainsKey(id))
        {
            return;
        }

        UIShopProduct shopProduct = shopProducts[id];
        shopProduct.transform.SetParent(null);
        DestroyImmediate(shopProduct.gameObject);
    }
}
