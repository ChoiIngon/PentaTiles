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
    public BlockTile blockTilePrefab;

    public ToggleButton pencilToggle;
    public ToggleButton eraserToggle;
    public Button createButton;

    private Color currentColor;
	private Vector3 position;
		
	public int blockID;
	public enum State {
		Invalid, Pencil, Eraser
	}
	public State state = State.Pencil;

	public List<MapTile> mapTiles;
	void Start () {
        Map.Instance.editMode = true;
        state = State.Invalid;
        blockID = 1;
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
	}

	public Block CreateBlock() {
		GameObject go = new GameObject ();
        Block block = go.AddComponent<Block>();

        block.id = blockID;
        block.gameObject.name = "Block_" + blockID;
        block.transform.position = mapTiles[0].transform.position;
        foreach (MapTile mapTile in mapTiles) {
            BlockTile blockTile = GameObject.Instantiate<BlockTile>(blockTilePrefab);
            blockTile.transform.position = mapTile.transform.position;
            blockTile.transform.SetParent(block.transform);
            mapTile.id = block.id;
        }
		block.tileColor = currentColor;
		block.Init ();
		block.transform.localPosition = Vector3.zero;

        mapTiles = new List<MapTile>();
        blockID++;
        state = State.Invalid;

        return block;
	}

	public void OnClickMapTile(MapTile mapTile)
	{
        if (State.Pencil == state)
        {
            if (0 == mapTiles.Count)
            {
                currentColor = colors[Random.Range(0, colors.Length)];
            }
            mapTile.spriteRenderer.color = currentColor;
            mapTiles.Add(mapTile);
        }
        else if (State.Eraser == state)
        {
            mapTile.spriteRenderer.color = Color.white;
            mapTiles.Remove(mapTile);
        }
	}

	public void OnClickBlockTile(BlockTile blockTile)
	{
		
	}

    public void Save()
    {

    }

    public void Load()
    {
    }
}
