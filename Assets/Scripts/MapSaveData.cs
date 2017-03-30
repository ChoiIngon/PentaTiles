using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapSaveData : ScriptableObject {
	public int stage;
	public int level;

	public int width;
	public int height;

	public int[] tiles;

    public float blockSlotScale;
	public BlockSaveData[] blocks;
}
