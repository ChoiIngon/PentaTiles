using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBlock : Block {
	public override void Init(BlockSaveData saveData)
	{
		base.Init (saveData);
		sortingOrder = (int)Block.SortingOrder.Slot;
		tileColor = Color.gray;
		name = saveData.name + "_Slot";
		transform.SetParent (Map.Instance.slots, false);
		transform.localScale = new Vector3(Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
		transform.localPosition = saveData.slotPosition;
		foreach (BlockTile tile in blockTiles) {
			tile.transform.localRotation = new Quaternion (0.0f, 0.0f, 180.0f, 0.0f);
		}

		if (true == Map.Instance.editMode) {
			enableTouchInput = true;
		} else {
			enableTouchInput = false;
		}
	}

#if UNITY_EDITOR
	public override void OnClick(Vector3 position) 
	{
		if(true == Map.Instance.editMode) 
		{
			if (Editor.State.Pencil == Editor.Instance.state)
			{
				sortingOrder = (int)Block.SortingOrder.Select;
			}
			else if (Editor.State.Eraser == Editor.Instance.state)
			{
				Editor.Instance.DestroyBlock(id);
			}
		}
	}
	public override void OnDrag(Vector3 delta)
	{
		if(true == Map.Instance.editMode)
		{
			if (Editor.State.Pencil == Editor.Instance.state)
			{
				transform.position += delta;
			}
		}
	}
	public override void OnDrop(Vector3 position)
	{
		if (true == Map.Instance.editMode)
		{
			if (Editor.State.Pencil == Editor.Instance.state)
			{
				sortingOrder = (int)Block.SortingOrder.Slot;
			}
		}
	}	
#endif
}
