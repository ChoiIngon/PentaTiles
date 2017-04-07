using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

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
	public float blockSlotScale;

	public Transform tiles;
	public Transform blocks;
	public Transform slots;
	public Transform hints;
	public MapTile mapTilePrefab;
	
    public Block blockPrefab;
	public MapTile[] mapTiles;
	public void Init(int stage, int level)
	{
		MapSaveData mapSaveData = Resources.Load<MapSaveData> (stage + "_" + level);
		Init(mapSaveData);
	}
	public void Init(MapSaveData info)
	{
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
				mapTile.Init (info.tiles [x + y * width]);
				mapTiles [x + y * width] = mapTile;

                if (true == Map.Instance.editMode)
                {
                    mapTile.spriteRenderer.color = Color.white;
                }
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
				Block block = GameObject.Instantiate<Block> (blockPrefab);
                block.Init(blockSaveData);
                block.slot.transform.localPosition = blockSaveData.slotPosition;
                block.slot.transform.localScale = new Vector3(Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);

                block.transform.position = block.slot.transform.position;
                block.transform.localScale = new Vector3(Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
				block.initPosition = block.transform.position;
                
				if (null != block.hint) {
					block.hint.transform.localPosition = blockSaveData.hintPosition;
					block.hint.gameObject.SetActive (false);

					if (true == editMode) {
						block.transform.localScale = Vector3.one;
						block.transform.position = block.hint.transform.position;
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

		BlockTile[] blockTiles = blocks.GetComponentsInChildren<BlockTile> ();
		foreach (BlockTile blockTile in blockTiles) {
			iTween.RotateBy(blockTile.gameObject, iTween.Hash("x", 20.0f, "time", 1.0f));
		}

        Analytics.CustomEvent("LevelComplete", new Dictionary<string, object> {
            {"stage", Game.Instance.playData.currentStage.stage },
            {"level", Game.Instance.playData.currentLevel}
        });
        return true;
	}

	public bool UseHint()
	{
		List<GameObject> candidates = new List<GameObject> ();
		for (int i = 0; i < hints.childCount; i++) {
			Transform candidate = hints.GetChild (i);
			if (false == candidate.gameObject.activeSelf) {
				candidates.Add (candidate.gameObject);
			}
		}

		if (0 == candidates.Count) {
			return false;
		}

        AudioManager.Instance.Play("HintUse");
        candidates [Random.Range (0, candidates.Count)].SetActive (true);
		
        Analytics.CustomEvent("HintUse", new Dictionary<string, object> {
            {"stage", Game.Instance.playData.currentStage.stage },
            {"level", Game.Instance.playData.currentLevel}
        });
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
		saveData.blocks = new BlockSaveData[blocks.childCount];
		for (int i = 0; i < blocks.childCount; i++) {
			saveData.blocks [i] = blocks.GetChild(i).GetComponent<Block>().GetSaveData();
		}
		for(int i=0; i<mapTiles.Length; i++)
		{
            saveData.tiles[i] = mapTiles[i].id;
		}
		return saveData;
	}
}

