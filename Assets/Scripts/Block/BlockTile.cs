using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockTile : MonoBehaviour {
	[ReadOnly]
	public Block block;

	public SpriteRenderer tileSprite;
	public SpriteRenderer outlineSprite;

    [HideInInspector]
    public TouchInput touchInput;
    [HideInInspector]
    public MapTile mapTile;
	public BoxCollider boxCollier;

	public bool enableTouchInput {
		set {
			touchInput.enabled = value;
			boxCollier.enabled = value;
		}
	}
		
	public void Init (Block block) {
        this.block = block;
		transform.SetParent (block.transform, false);
		tileSprite = transform.FindChild ("TileSprite").GetComponent<SpriteRenderer> ();
		outlineSprite = transform.FindChild ("OutlineSprite").GetComponent<SpriteRenderer>();
		outlineSprite.gameObject.SetActive (false);

		touchInput = GetComponent<TouchInput> ();
		touchInput.onTouchDown = null;
		touchInput.onTouchDown += (Vector3 position) =>
		{
			block.OnClick(position);
		};
		touchInput.onTouchDrag = null;
		touchInput.onTouchDrag += (Vector3 delta) =>
		{
			block.OnDrag(delta);
		};
		touchInput.onTouchUp = null;
		touchInput.onTouchUp += (Vector3 position) =>
		{
			Vector3 delta = Vector3.zero;
			if(null != mapTile)
			{
				delta = mapTile.transform.position - transform.position;
			}
			block.OnDrop(block.transform.position + delta);
		};

		boxCollier = GetComponent<BoxCollider> ();
    }

    void OnTriggerStay(Collider coll) {
		if ("MapTile" == coll.gameObject.tag) {
			MapTile markedMapTile = coll.gameObject.GetComponent<MapTile> ();

			if (false == Map.Instance.editMode && 0 == markedMapTile.id) {
				return;
			}
				
			if (true == coll.bounds.Contains(transform.position)) {
				markedMapTile.blockTile = this;
				mapTile = markedMapTile;
				foreach (BlockTile blockTile in block.blockTiles) {
					if (null == blockTile.mapTile) {
						return;
					}
				}
				mapTile.spriteRenderer.color = tileSprite.color;
			}
			else 
			{
				if (this == markedMapTile.blockTile) {
					markedMapTile.spriteRenderer.color = markedMapTile.color;
				}
				if (markedMapTile == mapTile) {
					mapTile = null;
				}
			}
		}
	}

	void OnTriggerExit(Collider coll) {
		if ("MapTile" == coll.gameObject.tag) {
			MapTile markedMapTile = coll.gameObject.GetComponent<MapTile> ();
			if (false == Map.Instance.editMode && 0 == markedMapTile.id) {
				return;
			}
			if (this == markedMapTile.blockTile) {
				markedMapTile.spriteRenderer.color = markedMapTile.color;
			}
			if (markedMapTile == mapTile) {
				mapTile = null;
			}

			bool clearColor = false;
			foreach (BlockTile blockTile in block.blockTiles) {
				if (null == blockTile.mapTile) {
					clearColor = true;
				}
			}
			if (true == clearColor) {
				foreach (BlockTile blockTile in block.blockTiles) {
					if (null != blockTile.mapTile) {
						blockTile.mapTile.spriteRenderer.color = markedMapTile.color;
					}
				}
			}
		}
	}
}
