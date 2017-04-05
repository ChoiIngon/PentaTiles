using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockSaveData : ScriptableObject {
	public int id;
    public Color tileColor;
    public List<Vector3> tilePositions; // relative position with block
	public Vector3 slotPosition;    // relative position with block slot
	public Vector3 hintPosition;    // relative position with map tile
}
