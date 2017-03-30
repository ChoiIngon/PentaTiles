using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
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

	public bool editMode;
	public int stage;
	public int level;
	public int width;
	public int height;
	[Range(0.0f, 1.0f)]
	public float blockSlotScale;
	private MapTile[] mapTiles;
	//public List<Block> blocks;

	public MapTile mapTilePrefab;
	public Block[] blockPrefabs;

    public Transform tiles;
	public Transform blocks;
	public Transform hints;

	void Start()
	{
		Init (null);
	}

	public void Init(MapSaveData info)
	{
		if (null == info) {
			info = ScriptableObject.CreateInstance<MapSaveData> ();
			info.level = Map.Instance.stage;
			info.stage = Map.Instance.level;
			info.width = Map.Instance.width;
			info.height = Map.Instance.height;
			info.tiles = new int[info.width * info.height];
		}

		while (0 < tiles.childCount) {
			Transform trMapTile = tiles.GetChild (0);
			trMapTile.SetParent (null);
			DestroyImmediate (trMapTile.gameObject);
		}
		while (0 < blocks.childCount) {
			Block block = blocks.GetChild (0).GetComponent<Block>();
			block.blockSlot.transform.SetParent (null);
			DestroyImmediate (block.blockSlot.gameObject);

			block.transform.SetParent (null);
			DestroyImmediate (block.gameObject);
		}
        while (0 < hints.childCount)
        {
            Transform hint = hints.GetChild(0);
            hint.SetParent(null);
            DestroyImmediate(hint.gameObject);
        }

		if (null == info) {
			throw new System.Exception ("null level info");
		}
		stage = info.stage;
		level = info.level;
		width = info.width;
		height = info.height;

		if (0 == width || 0 == height) {
			return;
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
			Transform blockSlots = transform.FindChild ("BlockSlots");
			Vector3 position = blockSlots.position;
			position.x = 0.0f;
			blockSlots.transform.position = position;
		}

		if (null != info.blocks) {
			foreach (BlockSaveData blockSaveData in info.blocks) {
				Block block = GameObject.Instantiate<Block> (blockPrefabs [blockSaveData.id - 1]);
				block.name = "Block_" + blockSaveData.id;
				block.transform.SetParent (blocks, false);
				block.transform.position = blockSaveData.slotPosition;
				block.Init ();                

				if (blockSaveData.slotPosition != blockSaveData.hintPosition) {
                    block.CreateHint();
                    block.hint.transform.position = blockSaveData.hintPosition;
                    if(true == editMode)
                    {
                        block.transform.position = blockSaveData.hintPosition;
                    }
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

    private void OnGUI()
    {
    }
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
		}

		if(GUILayout.Button("Load"))
		{
			MapSaveData data = Resources.Load<MapSaveData> (Map.Instance.stage + "_" + Map.Instance.level);
			Map.Instance.Init (data);
		}
	}
}
#endif