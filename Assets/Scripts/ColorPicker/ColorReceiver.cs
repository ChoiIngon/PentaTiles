using UnityEngine;

public class ColorReceiver : MonoBehaviour {
	private static ColorReceiver _instance;  
	public static ColorReceiver Instance {  
		get {  
			if (!_instance) {  
				_instance = (ColorReceiver)GameObject.FindObjectOfType(typeof(ColorReceiver));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "ColorReceiver";  
					_instance = container.AddComponent<ColorReceiver>();  
				}  
			}

			return _instance;  
		}  
	}
	public Color color = Color.black;

	void OnColorChange(HSBColor color) 
	{
		ColorReceiver.Instance.color = color.ToColor();
		ColorReceiver.Instance.color.a = 1.0f;
	}

    void OnGUI()
    {
		var r = Camera.main.pixelRect;
		var rect = new Rect(r.center.x + r.height / 6 + 50, r.center.y, 100, 100);
		GUI.Label (rect, "#" + ToHex(color.r) + ToHex(color.g) + ToHex(color.b) + ToHex(color.a));
    }

	string ToHex(float n)
	{
		return ((int)(n * 255)).ToString("X").PadLeft(2, '0');
	}
}
