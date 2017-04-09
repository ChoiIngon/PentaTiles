using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;
#endif
[System.Serializable]
public class MapSaveData : ScriptableObject {
	public int stage;
	public int level;

	public int width;
	public int height;

	public int[] tiles;

    public float blockSlotScale;
	public BlockSaveData[] blocks;

	#if UNITY_EDITOR
	[OnOpenAssetAttribute(1)]
	public static bool Open(int instanceID, int line)
	{
		if(true == Application.isPlaying && true == Map.Instance.editMode)
		{
			string name = EditorUtility.InstanceIDToObject(instanceID).name;
			MapSaveData data = Resources.Load<MapSaveData>(name);
			if (null == data) {
				return false;
			}
			Editor.Instance.Init();
			Map.Instance.Init(data);
			Editor.Instance.stage = data.stage;
			Editor.Instance.level = data.level;
			Editor.Instance.width = data.width;
			Editor.Instance.height = data.height;
			Editor.Instance.blockSlotScale = data.blockSlotScale;
			Editor.Instance.blockSlotScale = Map.Instance.blockSlotScale;
			for (int i = 0; i < Map.Instance.blocks.childCount; i++)
			{
				Block block = Map.Instance.blocks.GetChild(i).GetComponent<Block>();
				Editor.Instance.blocks.Add(block.id, block);
				Editor.Instance.blockID = Mathf.Max (Editor.Instance.blockID, block.id+1);
			}

			Debug.Log("success to load map file from " + "\'Assets/Resources/" + data.stage + "_" + data.level + ".asset" + "\'");
			return true;
		}
		return false;
	}
	#endif
}
