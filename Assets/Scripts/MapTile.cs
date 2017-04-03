using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	
	public int id;
	[HideInInspector] public bool editMode;
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
			spriteRenderer.color = deactiveColor;
		}

		if (true == editMode) {
			id = 0;
			spriteRenderer.color = activeColor;
		}
#if UNITY_EDITOR
		TouchInput touch = gameObject.AddComponent<TouchInput>();
		touch.onTouchUp += (Vector3 position) =>  {
			Debug.Log("touch(id:" + id + ")");
			spriteRenderer.color = ColorReceiver.Instance.color;
		};
#endif
	}
}
