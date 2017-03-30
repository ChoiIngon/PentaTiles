using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class Block : MonoBehaviour {
	public enum SortingOrder
	{
		Slot,
		Hint,
		Idle,
		PutOnTheMap,
		Select
	}
	public bool editMode;
	public int id;
	[HideInInspector] public float scale;

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
		editMode = Map.Instance.editMode;
		scale = Map.Instance.blockSlotScale;
		initPosition = transform.position;
		blockTiles = new List<BlockTile> ();
		for (int i = 0; i < transform.childCount; i++) {
			if (true == transform.GetChild (i).gameObject.activeSelf) {
				BlockTile blockTile = transform.GetChild (i).GetComponent<BlockTile> ();
				blockTile.Init ();
                blockTile.spriteRenderer.sortingOrder = (int)SortingOrder.Idle;
				blockTiles.Add (blockTile);
			}
		}
		mapTiles = new List<MapTile> ();
		CreateBlockSlot ();
	}

	public void OnClick() {
		transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
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
			initPosition = transform.position;
			foreach(MapTile mapTile in mapTiles)
			{
				mapTile.block = null;
				mapTile.id = 0;
			}
			mapTiles = new List<MapTile> ();
            if(null != hint)
            {
                DestroyImmediate(hint);
                hint = null;
            }
			return;
		}

		foreach(BlockTile blockTile in blockTiles) {
			if (null == blockTile.mapTile) {
				transform.position = initPosition;
				transform.localScale = new Vector3 (scale, scale, 1.0f);
				return;
			}

			MapTile mapTile = blockTile.mapTile;
			if (false == editMode && 0 == mapTile.id) {
				transform.position = initPosition;
				transform.localScale = new Vector3 (scale, scale, 1.0f);
				return;
			}
			if (null != mapTile.block && this != mapTile.block) {
				transform.position = initPosition;
				transform.localScale = new Vector3 (scale, scale, 1.0f);
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

        if (true == editMode)
        {
            CreateHint();
        }
        if (false == editMode) {
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
        }
        else
        {
            saveData.hintPosition = blockSlot.transform.position;
        }
		return saveData;
	}

	private void CreateBlockSlot() {
		if (null == blockSlot) {
            blockSlot = CloneTiles((int)SortingOrder.Slot);
			blockSlot.name = "BlockSlot";
		}
		blockSlot.transform.localScale = new Vector3 (scale, scale, 1.0f);
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
			Map map = block.transform.GetComponentInParent<Map> ();
			if (null != map) {
				block.editMode = map.editMode;
			}

			BlockSlots blockSlots = block.transform.GetComponentInParent<BlockSlots> ();
			if (null != blockSlots) {
				block.scale = blockSlots.scale;
				block.initPosition = block.transform.position;
				block.blockSlot.transform.position = block.transform.position;
				block.blockSlot.transform.localScale = new Vector3 (block.scale, block.scale, 1.0f);
			}
		}

		if (GUILayout.Button ("OnDrop")) {
			foreach (BlockTile blockTile in block.blockTiles) {
				blockTile.spriteRenderer.sortingOrder += 1;
			}
			block.OnDrop (block.transform.position);
		}
	}
}
#endif