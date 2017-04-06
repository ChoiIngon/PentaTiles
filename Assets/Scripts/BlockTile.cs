using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockTile : MonoBehaviour {
	[ReadOnly]
	public Block block;
    [ReadOnly]
	public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public TouchInput touchInput;
    [HideInInspector]
    public MapTile mapTile;
    private Color originalColor;
    private Color markColor;
    public void Init (Block block) {
        this.block = block;
        spriteRenderer.sortingOrder = (int)Block.SortingOrder.Idle;
        originalColor = spriteRenderer.color;
        markColor = spriteRenderer.color;
        markColor.a = 0.5f;
             
		touchInput = GetComponent<TouchInput> ();
		touchInput.onTouchDown = null;
		touchInput.onTouchDrag = null;
		touchInput.onTouchUp = null;

        touchInput.onTouchDown += (Vector3 position) =>
        {
            if (false == Map.Instance.editMode)
            {
                if (Block.Type.Block == block.type)
                {
                    block.OnClick();
                }
            }
#if UNITY_EDITOR
            else {
                if (Block.Type.Slot == block.type)
                {
                    if (Editor.State.Pencil == Editor.Instance.state)
                    {
                        foreach (BlockTile blockTile in block.blockTiles)
                        {
                            blockTile.spriteRenderer.sortingOrder = (int)Block.SortingOrder.Select;
                        }
                    }

                    else if (Editor.State.Eraser == Editor.Instance.state)
                    {
                        Editor.Instance.DestroyBlock(block.id);
                    }
                }

                else if (Block.Type.Block == block.type)
                {
                    if (null != block.hint)
                    {
                        block.hint.transform.SetParent(null);
                        DestroyImmediate(block.hint.gameObject);
                    }
                }
            }
#endif
        };

        touchInput.onTouchDrag += (Vector3 delta) =>
        {
            if (false == Map.Instance.editMode)
            {
                if (Block.Type.Block == block.type)
                {
                    block.transform.position += delta;
                }
            }
#if UNITY_EDITOR
            else {
                if (Block.Type.Slot == block.type)
                {
                    if (Editor.State.Pencil == Editor.Instance.state)
                    {
                        block.transform.position += delta;
                    }
                }
            }
#endif
        };

        touchInput.onTouchUp += (Vector3 position) =>
        {
            if (false == Map.Instance.editMode)
            {
                if (Block.Type.Block == block.type)
                {
                    Vector3 delta = Vector3.zero;
                    if (null != mapTile)
                    {
                        delta = mapTile.transform.position - transform.position;
                    }
                    block.OnDrop(block.transform.position + delta);
                }
            }
#if UNITY_EDITOR
#endif
        };

        if (false == Map.Instance.editMode)
        {
            if (Block.Type.Block != block.type)
            {
                touchInput.enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
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
                mapTile.spriteRenderer.color = markColor;
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
