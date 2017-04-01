using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockTile : MonoBehaviour {
	[HideInInspector]
	public Block block;
	public TouchInput touchInput;
	public MapTile mapTile;
	[HideInInspector]
	public SpriteRenderer spriteRenderer;
	private Color markingColor;
	public void Init () {
		block = GetComponentInParent<Block> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.sortingOrder = (int)Block.SortingOrder.Idle;
        markingColor = spriteRenderer.color / 2;
		touchInput = GetComponent<TouchInput> ();
		touchInput.onTouchDown = null;
		touchInput.onTouchDrag = null;
		touchInput.onTouchUp = null;
		touchInput.onTouchDown += (Vector3 position) => {
			block.OnClick();
		};
		touchInput.onTouchDrag += (Vector3 delta) => {
			block.transform.position += delta;
		};
		touchInput.onTouchUp += (Vector3 position) => {
			Vector3 delta = Vector3.zero;
			if(null != mapTile)
			{
				delta = mapTile.transform.position - transform.position;
			}

			block.OnDrop(block.transform.position + delta);
		};
	}

    void OnTriggerStay(Collider coll) {
		if ("MapTile" == coll.gameObject.tag) {
			MapTile markedMapTile = coll.gameObject.GetComponent<MapTile> ();

			if (false == Map.Instance.editMode && 0 == markedMapTile.id) {
				return;
			}
				
			if (true == coll.bounds.Contains(transform.position)) {
				markedMapTile.blockTile = this;
				markedMapTile.spriteRenderer.color = markingColor;
				mapTile = markedMapTile;
			}
			else 
			{
				if (this == markedMapTile.blockTile) {
					markedMapTile.spriteRenderer.color = markedMapTile.activeColor;
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
				markedMapTile.spriteRenderer.color = markedMapTile.activeColor;
			}
			if (markedMapTile == mapTile) {
				mapTile = null;
			}
		}
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(BlockTile))]
public class BlockTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BlockTile blockTile = (BlockTile)target;
        DrawDefaultInspector();
        if (true == Map.Instance.editMode)
        {
			if (blockTile.gameObject == Selection.activeGameObject && null != blockTile.block)
            {
                Selection.activeGameObject = blockTile.block.gameObject;
            }
        }
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif