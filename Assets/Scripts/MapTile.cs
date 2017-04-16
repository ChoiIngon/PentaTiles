using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	public int id;
	[HideInInspector] public BlockTile blockTile;
	[HideInInspector] public Block block;
	[HideInInspector] public SpriteRenderer spriteRenderer;

	public Color activeColor;
	public Color deactiveColor;

	public void Init(int id) {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		blockTile = null;
		block = null;

		this.id = id;
		if (0 != id) {
			spriteRenderer.color = activeColor;
		} else {
			//spriteRenderer.color = deactiveColor;
			gameObject.SetActive(false);
		}

#if UNITY_EDITOR
		if(true == Map.Instance.editMode) {
			gameObject.SetActive(true);
			spriteRenderer.color = Color.white;
			if(0 != id)	{
				spriteRenderer.color = Color.gray;
			}
			TouchInput touch = gameObject.AddComponent<TouchInput>();
			touch.onTouchUp += (Vector3 position) =>  {
				Editor.Instance.OnClickMapTile(this);
            };
		}
#endif
	}
}
