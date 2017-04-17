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
    
    public int stage;
    public int level;
    public int width;
    public int height;
    [Range(0, 1)]
    public float blockSlotScale;
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
    public Dictionary<int, MapBlock> blocks;
    
    void Start () {
        Init();
        Map.Instance.editMode = true;
        pencilToggle.onValueChanged.AddListener(value => {
			state = value ? State.Pencil : State.Eraser;	
			if(true == value)
			{
				currentColor = colors[Random.Range(0, colors.Length)];
				foreach(MapTile mapTile in mapTiles)
				{
					mapTile.spriteRenderer.color = currentColor;
				}
			}
		});
		eraserToggle.onValueChanged.AddListener(value => {
			state = value ? State.Eraser : State.Pencil;		
		});
		createButton.onClick.AddListener (() => {
			CreateBlock();
		});
		state = State.Pencil;
	}

    public void Init()
    {
        blockSlotScale = 1.0f;
        blockID = 1;
        mapTiles = new List<MapTile>();
        blocks = new Dictionary<int, MapBlock>();
		currentColor = colors[Random.Range(0, colors.Length)];
    }

    private Block CreateBlock() {
		BlockSaveData saveData = ScriptableObject.CreateInstance<BlockSaveData> ();
		saveData.id = blockID;
		saveData.name = "Block_" + blockID;
		saveData.tileColor = currentColor;
		saveData.slotPosition = Vector3.zero;
		saveData.hintPosition = mapTiles[0].transform.localPosition;
		saveData.tilePositions = new List<Vector3> ();
		foreach (MapTile mapTile in mapTiles) {
			saveData.tilePositions.Add(mapTile.transform.localPosition - saveData.hintPosition);
		}

		MapBlock block = GameObject.Instantiate<MapBlock> (Map.Instance.mapBlockPrefab);
		block.Init (saveData);
        blocks.Add(block.id, block);

        blockID++;
        mapTiles = new List<MapTile>();
		currentColor = colors[Random.Range(0, colors.Length)];
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
		Debug.Log ("destroy block(id:" + blockID + ")");
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(Editor))]
public class EditorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
		DrawDefaultInspector();
		if (false == Application.isPlaying) {
			return;
		}
        Editor editor = (Editor)target;
        
        if (GUILayout.Button("Init"))
        {
            MapSaveData mapSaveData = ScriptableObject.CreateInstance<MapSaveData>();
            mapSaveData.stage = editor.stage;
            mapSaveData.level = editor.level;
            mapSaveData.width = editor.width;
            mapSaveData.height = editor.height;
            mapSaveData.tiles = new int[mapSaveData.width * mapSaveData.height];
            mapSaveData.blockSlotScale = 1.0f;
            Map.Instance.Init(mapSaveData);
        }

        if (GUILayout.Button("Save"))
        {
			Map.Instance.stage = editor.stage;
			Map.Instance.level = editor.level;

            MapSaveData saveData = Map.Instance.GetSaveData();
            AssetDatabase.CreateAsset(saveData, "Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset");
            for (int i = 0; i < saveData.blocks.Length; i++)
            {
                AssetDatabase.AddObjectToAsset(saveData.blocks[i], saveData);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("success to save map file to " + "\'Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset" + "\'");
        }

        if (GUILayout.Button("Load"))
        {
            editor.Init();
            MapSaveData data = Resources.Load<MapSaveData>(editor.stage + "_" + editor.level);
            Map.Instance.Init(data);

			editor.blockSlotScale = Map.Instance.blockSlotScale;
            for (int i = 0; i < Map.Instance.blocks.childCount; i++)
            {
                MapBlock block = Map.Instance.blocks.GetChild(i).GetComponent<MapBlock>();
                editor.blocks.Add(block.id, block);
				editor.blockID = Mathf.Max (editor.blockID, block.id+1);
            }
            Debug.Log("success to load map file from " + "\'Assets/Resources/" + data.stage + "_" + data.level + ".asset" + "\'");
        }
		/*
		if (GUILayout.Button("Convert"))
		{
			for (int stage = 101; stage <= 170; stage++) {
				for (int level = 1; level <= 24; level++) {
					editor.Init ();
					MapSaveData data = Resources.Load<MapSaveData> (stage + "_" + level);
					if (null == data) {
						Debug.Log (stage + "_" + level + " is skip");
						continue;
					}

					Map.Instance.Init(data);
					editor.blockSlotScale = Map.Instance.blockSlotScale;
					for (int i = 0; i < Map.Instance.blocks.childCount; i++)
					{
						MapBlock block = Map.Instance.blocks.GetChild(i).GetComponent<MapBlock>();
						editor.blocks.Add(block.id, block);
						editor.blockID = Mathf.Max (editor.blockID, block.id+1);
					}
					Debug.Log("success to load map file from " + "\'Assets/Resources/" + data.stage + "_" + data.level + ".asset" + "\'");

					Map.Instance.stage = stage - 100;
					Map.Instance.level = level;

					MapSaveData saveData = Map.Instance.GetSaveData();
					AssetDatabase.CreateAsset(saveData, "Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset");
					for (int i = 0; i < saveData.blocks.Length; i++)
					{
						AssetDatabase.AddObjectToAsset(saveData.blocks[i], saveData);
					}
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();

					Debug.Log("success to save map file to " + "\'Assets/Resources/" + saveData.stage + "_" + saveData.level + ".asset" + "\'");
				}
			}
		}
		*/
        Map.Instance.blockSlotScale = editor.blockSlotScale;

        for (int i = 0; i < Map.Instance.slots.childCount; i++) {
			SlotBlock block = Map.Instance.slots.GetChild (i).GetComponent<SlotBlock>();
			block.transform.localScale = new Vector3 (Map.Instance.blockSlotScale, Map.Instance.blockSlotScale, 1.0f);
		}
    }
}
#endif
