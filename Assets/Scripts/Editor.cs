using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Editor : MonoBehaviour {
    public enum State
    {
        Pencil, Eraser
    }

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

	public Block blockPrefab;
    public ToggleButton pencilToggle;
    public ToggleButton eraserToggle;
    public Button createButton;
    [ReadOnly]
    public int blockID;
    [ReadOnly]
    public State state;
    public Color[] colors;
    private Color currentColor;
	private List<MapTile> mapTiles;
    public Dictionary<int, Block> blocks;
    
    void Start () {
        Init();
        Map.Instance.editMode = true;
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

    public void Init()
    {
        state = State.Pencil;
        blockID = 1;
        mapTiles = new List<MapTile>();
        blocks = new Dictionary<int, Block>();
    }

    private Block CreateBlock() {
		BlockSaveData saveData = ScriptableObject.CreateInstance<BlockSaveData> ();
		saveData.id = blockID;
		saveData.name = "Block_" + blockID;
		saveData.slotPosition = Vector3.zero;
		saveData.hintPosition = Vector3.zero;
		saveData.tileColor = currentColor;
		saveData.tilePositions = new List<Vector3> ();
		foreach (MapTile mapTile in mapTiles) {
			saveData.tilePositions.Add(mapTile.transform.localPosition);
		}

		Block block = GameObject.Instantiate<Block> (blockPrefab);
		block.Init (saveData);

        blocks.Add(block.id, block);

        blockID++;
        mapTiles = new List<MapTile>();
        return block;
	}

    public void DestroyBlock(int blockID)
    {
        foreach (MapTile mapTile in Map.Instance.mapTiles)
        {
            if (mapTile.id == blockID)
            {
                mapTile.spriteRenderer.color = Color.white;
                mapTile.id = 0;
            }
        }

        Block block = blocks[blockID];
        block.Destroy();
        blocks.Remove(blockID);
    }

	public void OnClickMapTile(MapTile mapTile)
	{
        if (State.Pencil == state)
        {
            if (0 != mapTile.id)
            {
                Debug.Log("already set tile(id:" + mapTile.id + ")");
                return;
            }
            if (0 == mapTiles.Count)
            {
                currentColor = colors[Random.Range(0, colors.Length)];
            }
            mapTile.id = blockID;
            mapTile.spriteRenderer.color = currentColor;
            mapTiles.Add(mapTile);
        }
        else if (State.Eraser == state)
        {
            mapTile.id = 0;
            mapTile.spriteRenderer.color = Color.white;
            mapTiles.Remove(mapTile);
        }
	}

    public void Save()
    {
    }

    public void Load()
    {
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Editor))]
public class EditorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        Editor editor = (Editor)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Init"))
        {
            Map.Instance.Init(null);
        }

        if (GUILayout.Button("Save"))
        {
            MapSaveData saveData = Map.Instance.GetSaveData();
            AssetDatabase.CreateAsset(saveData, "Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset");
            for (int i = 0; i < saveData.blocks.Length; i++)
            {
                AssetDatabase.AddObjectToAsset(saveData.blocks[i], saveData);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Map.Instance.dataPath = saveData.stage + "_" + saveData.level;

            Debug.Log("success to save map file to " + "\'Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset" + "\'");
        }

        if (GUILayout.Button("Load"))
        {
            editor.Init();
            MapSaveData data = Resources.Load<MapSaveData>(Map.Instance.stage + "_" + Map.Instance.level);
            Map.Instance.Init(data);
            for (int i = 0; i < Map.Instance.blocks.childCount; i++)
            {
                Block block = Map.Instance.blocks.GetChild(i).GetComponent<Block>();
                editor.blocks.Add(block.id, block);
            }
            Debug.Log("success to load map file from " + "\'Assets/Resources/" + data.stage + "_" + data.level + ".asset" + "\'");
        }

		for (int i = 0; i < Map.Instance.blocks.childCount; i++) {
			Block block = Map.Instance.blocks.GetChild (i).GetComponent<Block>();
			block.slot.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);

			if (block.transform.position == block.slot.transform.position) {
				block.transform.localScale = block.slot.transform.localScale;
			}
		}
    }

}
#endif
