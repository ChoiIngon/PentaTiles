using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        mapTiles = new List<MapTile>();

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
            //blockTile.spriteRenderer.color = saveData.tileColor;
            blockTile.Init (this);
            blockTiles.Add(blockTile);
        }

		slot = GameObject.Instantiate<Block>(this);
		if(null != slot)
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
                //Color color = saveData.tileColor / 2;
                //color.a = 1.0f;
                //blockTile.spriteRenderer.color = color;
                blockTile.Init(slot);
            }

            slot.transform.localPosition = saveData.slotPosition;
            slot.transform.localScale = new Vector3(Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
        }
		if (saveData.hintPosition != saveData.slotPosition) {
			hint = CreateHint ();
			hint.transform.localPosition = saveData.hintPosition;
		}
        
		if (true == Map.Instance.editMode && null != hint)
        {
			transform.position = hint.transform.position;
        }
        else
        {
            transform.position = slot.transform.position;
            transform.localScale = slot.transform.localScale;
            initPosition = transform.position;
        }
    }

	public int sortingOrder {
		set {
			foreach (BlockTile blockTile in blockTiles)
			{
				blockTile.spriteRenderer.sortingOrder = value;
			}
		}
	}
	public void OnClick() {
        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
		sortingOrder = (int)SortingOrder.Select;
        AudioManager.Instance.Play("BlockSelect");
	}

	public void OnDrop(Vector3 position) {
		sortingOrder = (int)SortingOrder.Idle;

		bool returnToSlotPosition = true;
		foreach(BlockTile blockTile in blockTiles) {
			if (null != blockTile.mapTile) {
				returnToSlotPosition = false;
				break;
			}
		}

		if (true == returnToSlotPosition) {
            transform.position = slot.transform.position;
            transform.localScale = slot.transform.localScale;
			initPosition = transform.position;
			foreach(MapTile mapTile in mapTiles)
			{
				mapTile.block = null;
			}
			mapTiles = new List<MapTile> ();
			if(true == Map.Instance.editMode)
            {
				foreach (MapTile mapTile in Map.Instance.mapTiles)
				{
					if (mapTile.id == id)
					{
						mapTile.spriteRenderer.color = Color.white;
						mapTile.id = 0;
					}
				}

				if (null != hint) {
					hint.transform.SetParent (null);
					DestroyImmediate (hint.gameObject);
					hint = null;
				}
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
			if (true == Map.Instance.editMode) {
				mapTile.id = 0;
			}
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
		if (true == Map.Instance.editMode) {
			hint = CreateHint ();
			hint.transform.position = transform.position;
		}
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

	public Block CreateHint()
	{
		if (null != hint) {
			return hint;
		}
		hint = GameObject.Instantiate<Block> (this);
		if (null != hint) {
			hint.name = name + "_Hint";
			hint.type = Type.Hint;
			hint.block = this;
			hint.slot = slot;
			hint.transform.SetParent (Map.Instance.hints, false);
			foreach (BlockTile blockTile in hint.blockTiles) {
				//Color color = tileColor;
				//color.a = 0.5f;
				//blockTile.spriteRenderer.color = color;
				blockTile.Init (hint);
			}
			hint.gameObject.SetActive(false);
		}
		return hint;
	}
}

