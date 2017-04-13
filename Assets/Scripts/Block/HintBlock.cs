using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBlock : Block {
	public override void Init(BlockSaveData saveData)
	{
		base.Init (saveData);
		transform.SetParent (Map.Instance.hints, false);
		transform.localPosition = saveData.hintPosition;
		enableTouchInput = false;
		name = saveData.name + "_Hint";
		Color color = saveData.tileColor;
		color.a = 0.5f;
		tileColor = color;
		sortingOrder = (int)Block.SortingOrder.Hint;

		gameObject.SetActive(false);
	}


	void OnEnable()
	{
		iTween.ScaleFrom (gameObject, new Vector3 (5.0f, 5.0f, 1.0f), 0.5f);
	}

	void Update()
	{
		Color color = tileColor;
		float alpha = 0.85f - ((Time.realtimeSinceStartup - (int)Time.realtimeSinceStartup)/2);
		color.a = alpha;
		foreach (BlockTile tile in blockTiles) {
			tile.tileSprite.color = color;
		}
	}
}
