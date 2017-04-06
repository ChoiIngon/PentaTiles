using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class Block : MonoBehaviour {
	public enum Type {
		Block,
		Hint,
		Slot
	}
	public enum SortingOrder
	{
		Slot,
		Hint,
		Idle,
		PutOnTheMap,
		Select
	}
	public Color tileColor;
	[ReadOnly] public int id;
	[ReadOnly] public Vector3 initPosition;
	[ReadOnly] public List<BlockTile> blockTiles;
    [ReadOnly] public Block block;
    [ReadOnly] public Block slot;
	[ReadOnly] public Block hint;
    [ReadOnly] public Type type;
	public BlockTile blockTilePrefab;
	public List<MapTile> mapTiles;

    public void Init(BlockSaveData saveData)
    {
        name = saveData.name;
        id = saveData.id;
		type = Type.Block;
        tileColor = saveData.tileColor;
		transform.SetParent (Map.Instance.blocks);
		transform.localPosition = saveData.tilePositions [0];
        blockTiles = new List<BlockTile>();
        foreach (Vector3 tilePosition in saveData.tilePositions)
        {
            BlockTile blockTile = GameObject.Instantiate<BlockTile>(blockTilePrefab);
			blockTile.transform.SetParent(Map.Instance.tiles);
            blockTile.transform.localPosition = tilePosition;
			blockTile.transform.SetParent(transform);
			blockTile.Init ();
			blockTile.spriteRenderer.color = saveData.tileColor;
            blockTiles.Add(blockTile);
        }

		hint = GameObject.Instantiate<Block> (this);
        slot = GameObject.Instantiate<Block>(this);

        {
            hint.name = name + "_Hint";
            hint.type = Type.Hint;
            hint.block = this;
            hint.slot = slot;
            hint.transform.SetParent(Map.Instance.hints, false);
            
            foreach (BlockTile blockTile in hint.blockTiles)
            {
                blockTile.Init();
            }
        }
        {
            slot.name = name + "_Slot";
            slot.type = Type.Slot;
            slot.block = this;
            slot.hint = hint;
            slot.transform.SetParent(Map.Instance.slots, false);
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localScale = new Vector3(Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
            foreach (BlockTile blockTile in slot.blockTiles)
            {
                blockTile.Init();
            }
        }

		initPosition = transform.position;
		mapTiles = new List<MapTile> ();
    }

    
	public void OnClick() {
        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        foreach (BlockTile blockTile in blockTiles)
        {
            blockTile.spriteRenderer.sortingOrder = (int)SortingOrder.Select;
        }
        AudioManager.Instance.Play("BlockSelect");
	}

	public void OnDrop(Vector3 position) {
		foreach (BlockTile blockTile in blockTiles) {
            blockTile.spriteRenderer.sortingOrder = (int)SortingOrder.Idle;
        }

		bool returnToOrigialPosition = true;
		foreach(BlockTile blockTile in blockTiles) {
			if (null != blockTile.mapTile) {
				returnToOrigialPosition = false;
				break;
			}
		}

		if (true == returnToOrigialPosition) {
            if (false == Map.Instance.editMode)
            {
                transform.position = slot.transform.position;
                transform.localScale = slot.transform.localScale;
            }
			initPosition = transform.position;
			foreach(MapTile mapTile in mapTiles)
			{
				mapTile.block = null;
			}
			mapTiles = new List<MapTile> ();
			if(true == Map.Instance.editMode && null != hint)
            {
                DestroyImmediate(hint);
                hint = null;
            }

			AudioManager.Instance.Play("BlockOut");
			return;
		}

		foreach(BlockTile blockTile in blockTiles) {
			if (null == blockTile.mapTile) {
				transform.position = initPosition;
				if (transform.position == slot.transform.position) {
					transform.localScale = slot.transform.localScale;
				}
				AudioManager.Instance.Play("BlockOut");
				return;
			}

			MapTile mapTile = blockTile.mapTile;
			if (false == Map.Instance.editMode && 0 == mapTile.id) {
				transform.position = initPosition;
				if (transform.position == slot.transform.position) {
					transform.localScale = slot.transform.localScale;
				}
				AudioManager.Instance.Play("BlockOut");
				return;
			}
			if (null != mapTile.block && this != mapTile.block) {
				transform.position = initPosition;
				if (transform.position == slot.transform.position) {
					transform.localScale = slot.transform.localScale;
				}
				AudioManager.Instance.Play("BlockOut");
				return;
			}
		}

		foreach(MapTile mapTile in mapTiles)
		{
			mapTile.block = null;
		}
		mapTiles = new List<MapTile> ();

		foreach(BlockTile blockTile in blockTiles) {
			MapTile mapTile = blockTile.mapTile;
			mapTile.block = this;
			mapTile.id = id;
			mapTiles.Add (mapTile);
		}

		initPosition = position;
		transform.position = position;

		if (false == Map.Instance.editMode) {
			StartCoroutine (Game.Instance.CompleteLevel ());
		}
		AudioManager.Instance.Play("BlockDrop");
	}

    public BlockSaveData GetSaveData()
	{
		BlockSaveData saveData = ScriptableObject.CreateInstance<BlockSaveData> ();
		saveData.name = name;
		saveData.id = id;
        saveData.tileColor = tileColor;
        saveData.tilePositions = new List<Vector3>();
        foreach (BlockTile blockTile in blockTiles)
        {
            saveData.tilePositions.Add(blockTile.transform.localPosition);
        }

		saveData.slotPosition = slot.transform.localPosition;
        if (null != hint) {
			saveData.hintPosition = hint.transform.localPosition;
			//OnDrop (transform.position);
        }
        else {
			saveData.hintPosition = saveData.slotPosition;
        }
		return saveData;
	}

    public void Destroy()
    {
        if(null != hint)
        {
            hint.transform.SetParent(null);
            DestroyImmediate(hint.gameObject);
        }
        if(null != slot)
        {
            slot.transform.SetParent(null);
            DestroyImmediate(slot.gameObject);
        }
        transform.SetParent(null);
        DestroyImmediate(this.gameObject);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Block)), CanEditMultipleObjects]
public class BlockEditor : UnityEditor.Editor 
{
	public override void OnInspectorGUI()
	{
		Block block = (Block)target;
		DrawDefaultInspector ();
		serializedObject.Update();
		//EditorGUILayout.PropertyField(lookAtPoint);
		serializedObject.ApplyModifiedProperties();
		if (GUILayout.Button ("Init")) {
            /*
			if (null != block.hint) {
				return;
			}
			block.Init ();
			block.slot.transform.position = block.transform.position;
			block.slot.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
			if (null != block.hint) {
				block.transform.localScale = block.slot.transform.localScale;
			}
            */
		}
	}
}
#endif