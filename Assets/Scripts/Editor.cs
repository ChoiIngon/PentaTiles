using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor : MonoBehaviour {
	private static Editor _instance;  
	public static Editor Instance {  
		get {  
			if (!_instance) {  
				_instance = (Editor)GameObject.FindObjectOfType(typeof(Editor));  
				if (!_instance) {  
					GameObject container = new GameObject();  
					container.name = "Editor";  
					_instance = container.AddComponent<Editor>();  
				}  
			}  
			return _instance;  
		}  
	}

	public Color[] colors;
	private Color currentColor;
	private Vector3 position;
	public ToggleButton pencilToggle;
	public ToggleButton eraserToggle;
	public Button createButton; 
	public BlockTile blockTilePrefab;
	public int blockID;
	public enum State {
		Invalid, Pencil, Eraser
	}
	public State state = State.Pencil;
	public List<BlockTile> blockTiles;
	public List<MapTile> mapTiles;
	void Start () {
		blockTiles = new List<BlockTile> ();
		mapTiles = new List<MapTile> ();
		pencilToggle.onValueChanged.AddListener(value => {
			state = value ? State.Pencil : State.Eraser;		
		});
		eraserToggle.onValueChanged.AddListener(value => {
			state = value ? State.Eraser : State.Pencil;		
		});
		createButton.onClick.AddListener (() => {
			CreateBlock();
		});
		Map.Instance.editMode = true;
	}

	public Block CreateBlock() {
		GameObject go = new GameObject ();
		go.name = "Block_" + (blockID++);

		Block block = go.AddComponent<Block> ();
		block.transform.SetParent (Map.Instance.blocks, false);
		block.transform.position = mapTiles [0].transform.position;
		block.tileColor = currentColor;
		foreach(MapTile mapTile in mapTiles) {
			BlockTile blockTile = GameObject.Instantiate<BlockTile> (blockTilePrefab);
			blockTile.transform.SetParent (block.transform, false);
			blockTile.transform.position = mapTile.transform.position;

		}
		block.Init ();
		block.transform.localPosition = Vector3.zero;
		return block;
	}

	public void OnClickMapTile(MapTile mapTile)
	{
		if (0 == mapTiles.Count) {
			currentColor = colors [Random.Range (0, colors.Length)];
		}
		mapTile.spriteRenderer.color = currentColor;
		mapTiles.Add (mapTile);
	}

	public void OnClickBlockTile(BlockTile blockTile)
	{
		if (State.Eraser == state) {
			blockTiles.Remove (blockTile);
			DestroyImmediate (blockTile.gameObject);
		}
	}
}
