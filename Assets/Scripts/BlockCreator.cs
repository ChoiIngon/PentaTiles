using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCreator : MonoBehaviour {
	public ToggleButton pencilToggle;
	public ToggleButton eraserToggle;
	public Button createButton; 
	public BlockTile blockTilePrefab;
	public enum State {
		Pencil, Eraser
	}
	public State state = State.Pencil;
	void Start () {
		pencilToggle.onValueChanged.AddListener(value => {
			state = value ? State.Pencil : State.Eraser;		
		});
		eraserToggle.onValueChanged.AddListener(value => {
			state = value ? State.Eraser : State.Pencil;		
		});
		createButton.onClick.AddListener (() => {
			Create();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Block Create() {
		GameObject go = new GameObject ();
		go.name = "Block";
		Block block = go.AddComponent<Block> ();

		for (int i = 0; i < 5; i++) {
			BlockTile blockTile = GameObject.Instantiate<BlockTile> (blockTilePrefab);
			blockTile.transform.SetParent (block.transform);
		}
		return block;
	}

}
