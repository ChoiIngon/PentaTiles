using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class Block : MonoBehaviour {
	public enum SortingOrder
	{
		Slot,
		Hint,
		Idle,
		PutOnTheMap,
		Select
	}
	public int id;

	public Vector3 initPosition;
	public List<BlockTile> blockTiles;
	private List<MapTile> mapTiles;

	public GameObject blockSlot;
    public GameObject hint;

	void Start () {
        if (Map.Instance.blocks == transform.parent)
        {
            return;
        }
		Init ();
	}
	public void Init()
	{
		transform.SetParent (Map.Instance.blocks, false);
		transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);

		initPosition = transform.position;
		blockTiles = new List<BlockTile> ();
		for (int i = 0; i < transform.childCount; i++) {
            BlockTile blockTile = transform.GetChild(i).GetComponent<BlockTile>();
            if (true == blockTile.gameObject.activeSelf) {
				blockTile.Init ();
				blockTiles.Add (blockTile);
			}       
		}
		mapTiles = new List<MapTile> ();
		CreateBlockSlot ();
	}

	public void OnClick() {
		transform.localScale = Vector3.one;
		transform.position = new Vector3 (transform.position.x, transform.position.y + 1.0f, transform.position.z);
		foreach (BlockTile blockTile in blockTiles) {
            blockTile.spriteRenderer.sortingOrder = (int)SortingOrder.Select;
        }
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
			transform.position = blockSlot.transform.position;		
			transform.localScale = blockSlot.transform.localScale;
			initPosition = transform.position;
			foreach(MapTile mapTile in mapTiles)
			{
				mapTile.block = null;
				//if (true == Map.Instance.editMode) {
				//	mapTile.id = 0;
				//}
			}
			mapTiles = new List<MapTile> ();
			if(true == Map.Instance.editMode && null != hint)
            {
                DestroyImmediate(hint);
                hint = null;
            }

			return;
		}

		foreach(BlockTile blockTile in blockTiles) {
			if (null == blockTile.mapTile) {
				transform.position = initPosition;
				if (transform.position == blockSlot.transform.position) {
					transform.localScale = blockSlot.transform.localScale;
				}
				return;
			}

			MapTile mapTile = blockTile.mapTile;
			if (false == Map.Instance.editMode && 0 == mapTile.id) {
				transform.position = initPosition;
				if (transform.position == blockSlot.transform.position) {
					transform.localScale = blockSlot.transform.localScale;
				}
				return;
			}
			if (null != mapTile.block && this != mapTile.block) {
				transform.position = initPosition;
				if (transform.position == blockSlot.transform.position) {
					transform.localScale = blockSlot.transform.localScale;
				}
				return;
			}
		}

		foreach(MapTile mapTile in mapTiles)
		{
			mapTile.block = null;
			//if (true == Map.Instance.editMode) {
			//	mapTile.id = 0;
			//}
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

		if (true == Map.Instance.editMode)
        {
            CreateHint();
        }
		if (false == Map.Instance.editMode) {
			StartCoroutine (Game.Instance.CheckCompleteStage ());
		}
	}

	public BlockSaveData GetSaveData()
	{
		BlockSaveData saveData = ScriptableObject.CreateInstance<BlockSaveData> ();
		saveData.name = name;
		saveData.id = id;
		saveData.slotPosition = blockSlot.transform.position;
        if (null != hint)
        {
            saveData.hintPosition = hint.transform.position;
			OnDrop (transform.position);
        }
        else
        {
			saveData.hintPosition = saveData.slotPosition;
        }
		return saveData;
	}

	private void CreateBlockSlot() {
		if (null == blockSlot) {
            blockSlot = CloneTiles((int)SortingOrder.Slot);
			blockSlot.name = "BlockSlot";
			blockSlot.transform.SetParent (Map.Instance.slots);
		}
		blockSlot.transform.localScale = transform.localScale;
		blockSlot.transform.position = transform.position;
	}

    public void CreateHint()
    {
        if (null == hint)
        {
            hint = CloneTiles((int)SortingOrder.Hint);
            hint.name = name + "_Hint";
            hint.transform.SetParent(Map.Instance.hints);
        }
        hint.transform.position = transform.position;
    }

    private GameObject CloneTiles(int sortingOrder)
    {
        GameObject cloneBlock = new GameObject();
        foreach (BlockTile blockTile in blockTiles)
        {
            GameObject hintTile = new GameObject();
            hintTile.name = "Tile";
            hintTile.transform.localPosition = blockTile.transform.localPosition;
            hintTile.transform.SetParent(cloneBlock.transform, false);
            SpriteRenderer spriteRenderer = hintTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = blockTile.spriteRenderer.sprite;
            spriteRenderer.color = new Color(
                blockTile.spriteRenderer.color.r / 2,
                blockTile.spriteRenderer.color.g / 2,
                blockTile.spriteRenderer.color.b / 2,
                1.0f
            );
            spriteRenderer.sortingLayerID = blockTile.spriteRenderer.sortingLayerID;
            spriteRenderer.sortingOrder = sortingOrder;
        }
        return cloneBlock;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Block)), CanEditMultipleObjects]
public class BlockEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Block block = (Block)target;
		DrawDefaultInspector ();
		serializedObject.Update();
		//EditorGUILayout.PropertyField(lookAtPoint);
		serializedObject.ApplyModifiedProperties();
		if (GUILayout.Button ("Init")) {
			if (null != block.hint) {
				return;
			}
			block.Init ();
			block.blockSlot.transform.position = block.transform.position;
			block.blockSlot.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
			if (null != block.hint) {
				block.transform.localScale = block.blockSlot.transform.localScale;
			}
		}
	}
}
#endif