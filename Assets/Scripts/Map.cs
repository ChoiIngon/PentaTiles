using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public MapBlock mapBlockPrefab;
	public HintBlock hintBlockPrefab;
	public SlotBlock slotBlockPrefab;
	[HideInInspector]
	public MapTile[] mapTiles;
	private List<MapBlock> mapBlocks;
	private List<HintBlock> activatedHintBlocks;
	private Coroutine _blinkHintBlock;

	public bool enableTouchInput 
	{
		set {
			if (null == mapBlocks) {
				return;
			}
			foreach (MapBlock mapBlock in mapBlocks) {
				mapBlock.enableTouchInput = value;
			}
		}
	}
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

		if (null != _blinkHintBlock) {
			StopCoroutine (_blinkHintBlock);
			_blinkHintBlock = null;
		}
		activatedHintBlocks = new List<HintBlock> ();
      
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

		mapBlocks = new List<MapBlock> ();
		if (null != info.blocks) {
			foreach (BlockSaveData blockSaveData in info.blocks) {
				MapBlock mapBlock = GameObject.Instantiate<MapBlock> (mapBlockPrefab);
				mapBlock.Init(blockSaveData);
				mapBlocks.Add (mapBlock);
            }
		}
	}
		
	public bool CheckComplete() {
		foreach (MapTile mapTile in mapTiles) {
			if (0 != mapTile.id && null == mapTile.block) {
				return false;
			}
		}

		enableTouchInput = false;

		foreach (MapTile mapTile in mapTiles) {
			mapTile.spriteRenderer.color = mapTile.activeColor;
		}

		foreach (MapBlock mapBlock in mapBlocks) {
			mapBlock.RotateTiles ();
			if (null != mapBlock.hint) {
				mapBlock.hint.gameObject.SetActive (false);
			}
		}
			
		if (null != _blinkHintBlock) {
			StopCoroutine (_blinkHintBlock);
			_blinkHintBlock = null;
		}


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
	
		HintBlock hintBlock = candidates [Random.Range (0, candidates.Count)].GetComponent<HintBlock> ();
		hintBlock.gameObject.SetActive (true);
		activatedHintBlocks.Add (hintBlock);

		if (null == _blinkHintBlock) {
			_blinkHintBlock = StartCoroutine (BlinkHintBlock ());
		}
        return true;
	}

	private IEnumerator BlinkHintBlock()
	{
		float blinkTime = 1.5f;
		const float max = 0.8f;
		const float min = 0.4f;
		float alpha = max;

		while (true) {
			alpha = max;
			while (min <= alpha) {
				alpha -= Time.deltaTime / blinkTime;
				foreach (HintBlock hintBlock in activatedHintBlocks) {
					Color color = hintBlock.tileColor;
					color.a = alpha;
					hintBlock.tileColor = color;
				}
				yield return null;
			}
			alpha = min;
			foreach (HintBlock hintBlock in activatedHintBlocks) {
				Color color = hintBlock.tileColor;
				color.a = alpha;
				hintBlock.tileColor = color;
			}
			yield return null;
			while (max >= alpha) {
				alpha += Time.deltaTime / blinkTime;
				foreach (HintBlock hintBlock in activatedHintBlocks) {
					Color color = hintBlock.tileColor;
					color.a = alpha;
					hintBlock.tileColor = color;
				}
				yield return null;
			}
			alpha = max;
			foreach (HintBlock hintBlock in activatedHintBlocks) {
				Color color = hintBlock.tileColor;
				color.a = alpha;
				hintBlock.tileColor = color;
			}
			yield return null;
		}
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
			saveData.blocks [i] = blocks.GetChild(i).GetComponent<MapBlock>().GetSaveData();
		}
		for(int i=0; i<mapTiles.Length; i++)
		{
            saveData.tiles[i] = mapTiles[i].id;
		}
		return saveData;
	}
}

