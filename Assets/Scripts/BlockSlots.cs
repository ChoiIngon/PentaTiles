using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockSlots : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Transform trBlocks = transform.FindChild ("Blocks");
		for (int i = 0; i < trBlocks.childCount; i++) {
			Block block = trBlocks.GetChild (i).GetComponent<Block> ();
			if (null == block) {
				continue;
			} 
		}
	}
}
	