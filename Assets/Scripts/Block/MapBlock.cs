using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : Block {
	[ReadOnly] public Vector3 initPosition;
	[ReadOnly] public SlotBlock slot;
	[ReadOnly] public HintBlock hint;
	[ReadOnly] public List<MapTile> mapTiles;

	public override void Init(BlockSaveData saveData)
	{
		base.Init (saveData);
		sortingOrder = (int)Block.SortingOrder.Idle;
		mapTiles = new List<MapTile>();

		transform.SetParent (Map.Instance.blocks, false);
		transform.localPosition = saveData.tilePositions [0];

		slot = GameObject.Instantiate<SlotBlock>(Map.Instance.slotBlockPrefab);
		slot.Init (saveData);

		if (saveData.hintPosition != saveData.slotPosition) {
			hint = GameObject.Instantiate<HintBlock> (Map.Instance.hintBlockPrefab);
			hint.Init (saveData);
		}

		transform.position = slot.transform.position;
		transform.localScale = slot.transform.localScale;
		initPosition = transform.position;

		if (true == Map.Instance.editMode && null != hint)
		{
			transform.position = hint.transform.position;
			transform.localScale = Vector3.one;
			initPosition = hint.transform.position;
		}
	}

	public override void OnClick(Vector3 position) {
		if (false == Map.Instance.editMode) {
			transform.localScale = Vector3.one;
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1.5f, transform.position.z);
			sortingOrder = (int)SortingOrder.Select;
			outline = true;
			AudioManager.Instance.Play ("BlockSelect");
		}
#if UNITY_EDITOR
		else {
			if (Editor.State.Eraser == Editor.Instance.state)
			{
				Editor.Instance.DestroyBlock(id);
			}
		}
#endif
	}

	public override void OnDrag(Vector3 delta)
	{
		if (false == Map.Instance.editMode)
		{
			transform.position += delta;
		}
#if UNITY_EDITOR
		else {
			if (Editor.State.Pencil == Editor.Instance.state)
			{
				 transform.position += delta;
			}
		}
#endif
	}
	public override void OnDrop(Vector3 position) {
		sortingOrder = (int)SortingOrder.Idle;
		outline = false;
		bool returnToSlotPosition = true;
		foreach(BlockTile blockTile in blockTiles) {
			if (null != blockTile.mapTile) {
				returnToSlotPosition = false;
				break;
			}
		}

		if (true == returnToSlotPosition) {
			initPosition = slot.transform.position;
			iTween.MoveTo (gameObject, slot.transform.position, 0.5f);
			transform.localScale = slot.transform.localScale;

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
				iTween.MoveTo (gameObject, initPosition, 0.2f);
				if (initPosition == slot.transform.position) {
					transform.localScale = slot.transform.localScale;
				}
				AudioManager.Instance.Play("BlockOut");
				return;
			}

			MapTile mapTile = blockTile.mapTile;
			if (false == Map.Instance.editMode && 0 == mapTile.id) {
				iTween.MoveTo (gameObject, initPosition, 0.2f);
				if (initPosition == slot.transform.position) {
					transform.localScale = slot.transform.localScale;
				}
				AudioManager.Instance.Play("BlockOut");
				return;
			}
			if (null != mapTile.block && this != mapTile.block) {
				iTween.MoveTo (gameObject, initPosition, 0.2f);
				if (initPosition == slot.transform.position) {
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

		Game.Instance.moveCount++;

		initPosition = position;
		transform.position = position;
		if (true == Map.Instance.editMode) {
			hint.transform.position = transform.position;
		}
		if (false == Map.Instance.editMode) {
			Game.Instance.CheckLevelComplete ();
		}
		AudioManager.Instance.Play("BlockDrop");
	}

	public override void Destroy()
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

	public void RotateTiles()
	{
		foreach (BlockTile blockTile in blockTiles) {
			iTween.RotateBy(blockTile.gameObject, iTween.Hash("x", 20.0f, "time", 1.0f));
		}
	}
}

