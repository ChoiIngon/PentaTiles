using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockSlots : MonoBehaviour {
	//public bool editMode;
	[Range(0.0f, 1.0f)]
	public float scale;

	// Use this for initialization
	void Start () {
		Transform trBlocks = transform.FindChild ("Blocks");
		for (int i = 0; i < trBlocks.childCount; i++) {
			Block block = trBlocks.GetChild (i).GetComponent<Block> ();
			if (null == block) {
				continue;
			} 
			block.scale = scale;
			block.transform.localScale = new Vector3 (scale, scale, 1.0f);
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(BlockSlots))]
public class BlockSlotsEditor : Editor 
{
	void OnEnable()
	{
		BlockSlots blockSlots = (BlockSlots)target;

	}

	public override void OnInspectorGUI()
	{
		BlockSlots blockSlots = (BlockSlots)target;
		DrawDefaultInspector ();

		Transform trBlocks = blockSlots.transform.FindChild ("Blocks");
		for (int i = 0; i < trBlocks.childCount; i++) {
			Block block = trBlocks.GetChild (i).GetComponent<Block> ();
			if (null == block) {
				continue;
			} 
			block.scale = blockSlots.scale;
			block.transform.localScale = new Vector3 (blockSlots.scale, blockSlots.scale, 1.0f);
		}
	}
}
#endif