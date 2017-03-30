using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

public class Map : MonoBehaviour {
	private static Map _instance;  
	public static Map Instance {  
		get {  
			if (!_instance) {  
				_instance = (Map)GameObject.FindObjectOfType(typeof(Map));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "Map";  
					_instance = container.AddComponent<Map>();  
				}  
			}  
			return _instance;  
		}  
	}

	[ReadOnly]
	public bool editMode;
	public int stage;
	public int level;
	public int width;
	public int height;
	[Range(0.0f, 1.0f)]
	public float blockSlotScale;

	public Transform tiles;
	public Transform blocks;
	public Transform slots;
	public Transform hints;
	public MapTile mapTilePrefab;
	public Block[] blockPrefabs;

	private MapTile[] mapTiles;
	#if UNITY_EDITOR
	public string dataPath;
	#endif

	public void Init(MapSaveData info)
	{
		editMode = false;
		if ("Editor" == SceneManager.GetActiveScene ().name) {
			editMode = true;
		}

		if (null == info) {
			#if UNITY_EDITOR
			dataPath = "none";
			#endif
			info = ScriptableObject.CreateInstance<MapSaveData> ();
			info.stage = stage;
			info.level = level;
			info.width = Mathf.Max(Map.Instance.width, 3);
			info.height = Mathf.Max(Map.Instance.height, 3);
			info.tiles = new int[info.width * info.height];
			info.blockSlotScale = 1.0f;
		}
		#if UNITY_EDITOR
		else {
			dataPath = info.stage + "_" + info.level;
		}
		#endif

		while (0 < tiles.childCount) {
			Transform tile = tiles.GetChild (0);
			tile.SetParent (null);
			DestroyImmediate (tile.gameObject);
		}

		while (0 < blocks.childCount) {
			Transform block = blocks.GetChild (0);
			block.SetParent (null);
			DestroyImmediate (block.gameObject);
		}

		while (0 < slots.childCount) {
			Transform slot = slots.GetChild (0);
			slot.SetParent (null);
			DestroyImmediate (slot.gameObject);
		}

		while (0 < hints.childCount) {
			Transform hint = hints.GetChild (0);
			hint.SetParent (null);
			DestroyImmediate (hint.gameObject);
		}
      
		stage = info.stage;
		level = info.level;
		width = info.width;
		height = info.height;
		blockSlotScale = info.blockSlotScale;
		if (0 == width || 0 == height) {
			throw new System.Exception ("zero size map");
		}

		mapTiles = new MapTile[width * height];
		for(int y = 0; y<height; y++)	{
			for (int x = 0; x<width; x++) {
				MapTile mapTile = GameObject.Instantiate<MapTile> (mapTilePrefab);
				mapTile.name = "MapTile_" + x + "_" + y;
				mapTile.transform.SetParent (tiles, false);
				mapTile.transform.localPosition = new Vector3 (x, height - y, 0.0f);
				mapTile.editMode = editMode;
				if (false == editMode) {
					mapTile.Init (info.tiles [x + y * width]);
				} else {
					mapTile.Init (0);
				}
				mapTiles [x + y * width] = mapTile;
			}
		}
		{
			Vector3 position = transform.position;
			position.x = -(float)(width - 1) / 2;
			transform.position = position;
		}

		{
			Transform blockSlots = transform.FindChild("BlockSlots");
			Vector3 position = blockSlots.position;
			position.x = 0.0f;
			blockSlots.transform.position = position;
		}

		if (null != info.blocks) {
			foreach (BlockSaveData blockSaveData in info.blocks) {
				Block block = GameObject.Instantiate<Block> (blockPrefabs [blockSaveData.id - 1]);
				block.name = "Block_" + blockSaveData.id;
				block.Init ();
				block.transform.SetParent (blocks, false);
				block.transform.position = blockSaveData.slotPosition;
				block.blockSlot.transform.position = blockSaveData.slotPosition;
				block.blockSlot.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);

				if (blockSaveData.slotPosition != blockSaveData.hintPosition) {
					block.CreateHint();
					block.hint.transform.position = blockSaveData.hintPosition;
					block.hint.SetActive (false);
				}

				if (true == editMode) {
					if (blockSaveData.slotPosition == blockSaveData.hintPosition) {
						block.transform.localScale = block.blockSlot.transform.localScale;
					}
					block.transform.position = blockSaveData.hintPosition;
				}
			}
		}
	}
		
	public bool CheckComplete() {
		foreach (MapTile mapTile in mapTiles) {
			if (0 != mapTile.id && null == mapTile.block) {
				return false;
			}
		}
		return true;
	}

	public MapSaveData GetSaveData()
	{
		MapSaveData saveData = ScriptableObject.CreateInstance<MapSaveData> ();
		saveData.stage = stage;
		saveData.level = level;
		saveData.width = width;
		saveData.height = height;
        saveData.blockSlotScale = blockSlotScale;
		saveData.tiles = new int[mapTiles.Length];
		for(int i=0; i<mapTiles.Length; i++)
		{
			if (null != mapTiles [i].block) {
				saveData.tiles [i] = mapTiles [i].block.id;
			} else {
				saveData.tiles [i] = 0;
			}
		}
			
		saveData.blocks = new BlockSaveData[blocks.childCount];
		for (int i = 0; i < blocks.childCount; i++) {
			saveData.blocks [i] = blocks.GetChild(i).GetComponent<Block>().GetSaveData();
		}

		return saveData;
	}
	#if UNITY_EDITOR
    private void OnGUI()
    {
		string text = "";
		text += "Stage : " + Map.Instance.stage + ", Level : " + Map.Instance.level + "\n";
		text += "Data file path : " + Map.Instance.dataPath + "\n";
		text += "Mode : " + (true == editMode ? "Edit" : "Game") + "\n";
		text += "Map Size :" + Map.Instance.width + " x " + Map.Instance.height + "\n";
		GUI.Label (new Rect (0, 0, 400, 100), text);
    }
	#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Map))]
public class MapEditor : Editor {
	void OnEnable() {
		Map map = (Map)target;
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if(GUILayout.Button("Init"))
		{
			Map.Instance.editMode = true;
			Map.Instance.Init(null);
		}

		if(GUILayout.Button("Save"))
		{
			MapSaveData saveData = Map.Instance.GetSaveData ();
			AssetDatabase.CreateAsset (saveData, "Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset");
			for (int i = 0; i < saveData.blocks.Length; i++) {
				AssetDatabase.AddObjectToAsset (saveData.blocks [i], saveData);
			}
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			Map.Instance.dataPath = saveData.stage + "_" + saveData.level;
		}

		if(GUILayout.Button("Load"))
		{
			MapSaveData data = Resources.Load<MapSaveData> (Map.Instance.stage + "_" + Map.Instance.level);
			Map.Instance.Init (data);
		}

		for (int i = 0; i < Map.Instance.blocks.childCount; i++) {
			Block block = Map.Instance.blocks.GetChild (i).GetComponent<Block>();
			block.blockSlot.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);

			if (block.transform.position == block.blockSlot.transform.position) {
				block.transform.localScale = block.blockSlot.transform.localScale;
			}
		}
	}
	/*
	[OnOpenAssetAttribute(1)]
	public static bool AutoOpen(int instanceID, int line)
	{
		if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(BlockSaveData))
		{
			string path = AssetDatabase.GetAssetPath(instanceID);
			Debug.Log ("open asset");
			return true;
		}
		return false;
	}
	*/
}
#endif