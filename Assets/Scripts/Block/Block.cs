using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	public enum SortingOrder
	{
		Slot = 10,
		Hint = 20,
		Idle = 30,
		Select = 40
	}

	[ReadOnly] public int id;
	[ReadOnly] public List<BlockTile> blockTiles;

	public BlockTile blockTilePrefab;

	public Color tileColor {
		set {
			foreach (BlockTile blockTile in blockTiles)	{
				blockTile.tileSprite.color = value;
			}
		}
		get {
			return blockTiles [0].tileSprite.color;
		}
	}
	public Color outlineColor {
		set {
			foreach (BlockTile blockTile in blockTiles)
			{
				blockTile.outlineSprite.color = value;
			}
		}
	}
	public int sortingOrder {
		set {
			foreach (BlockTile blockTile in blockTiles)
			{
				blockTile.tileSprite.sortingOrder = value;
				blockTile.outlineSprite.sortingOrder = value - 1;
			}
		}
	}
	public bool outline {
		set {
			foreach (BlockTile blockTile in blockTiles)
			{
				blockTile.outlineSprite.gameObject.SetActive (value);
			}
		}
	}

	public bool enableTouchInput {
		set {
			foreach (BlockTile blockTile in blockTiles)
			{
				blockTile.enableTouchInput = value;
			}
		}
	}

	public virtual void Init(BlockSaveData saveData) {
		blockTiles = new List<BlockTile>();
		foreach (Vector3 tilePosition in saveData.tilePositions) {
			BlockTile blockTile = GameObject.Instantiate<BlockTile> (blockTilePrefab);
			blockTile.transform.localPosition = tilePosition;
			blockTile.Init (this);
			blockTiles.Add (blockTile);
		}

		id = saveData.id;
		name = saveData.name;
		tileColor = saveData.tileColor;
	}

	public virtual void OnClick(Vector3 position) {}
	public virtual void OnDrag(Vector3 delta) {}
	public virtual void OnDrop(Vector3 position) {}
	public virtual void Destroy() {}


}

