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
	public Block blockPrefab;

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
		BlockSaveData saveData = ScriptableObject.CreateInstance<BlockSaveData> ();
		saveData.id = blockID;
		saveData.name = "Block_" + blockID;
		saveData.slotPosition = Vector3.zero;
		saveData.hintPosition = Vector3.zero;
		saveData.tileColor = currentColor;
		saveData.tilePositions = new List<Vector3> ();
		foreach (MapTile mapTile in mapTiles) {
			saveData.tilePositions.Add(mapTile.transform.localPosition);
			mapTile.id = blockID;
		}

		Block block = GameObject.Instantiate<Block> (blockPrefab);
		block.Init (saveData);

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
