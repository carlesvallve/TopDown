using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public TileStates state;
	private int stateNum;

	public TileSprites[] tileSprites;
	public Dictionary<TileStates, Sprite> sprites = new Dictionary<TileStates, Sprite>();


	public TileTypes type { get; set; }
	public int x { get; set; }
	public int y { get; set; }

	private GameGrid grid;
	private SpriteRenderer img;


	public void Init (GameGrid grid, TileTypes type, int x, int y) {
		this.grid = grid;
		this.type = type;
		this.x = x;
		this.y = y;

		// locate tile
		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, y * ratio, 0);

		// create images dictionary: images will be accessible by state key
		for (int i = 0; i < tileSprites.Length; i++) {
			sprites.Add(tileSprites[i].state, tileSprites[i].sprite);
		}

		// set tile image
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		img.sortingOrder = grid.height - y;

		// set tile state
		SetState(TileStates.NORMAL);
	}


	public bool IsWalkable () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null && (entity is Obstacle)) { return false; }
		if (type == TileTypes.WATER) { return false; }
		return true;
	}


	public bool IsOccupied () {
		Entity entity = grid.GetEntity(x, y);
		if (entity != null) { return false; }
		if (type == TileTypes.WATER) { return true; }
		return false;
	}


	public void SetState (TileStates state) {
		this.state = state;

		if (sprites.Count > 0) {
			img.sprite = sprites[state];
		}
	}


	public void UpdateBrokenState () {
		stateNum += 1;

		if (stateNum > System.Enum.GetValues(typeof(TileStates)).Length - 1) {
			grid.ChangeTile(this, TileTypes.WATER);
			return;
		}

		SetState((TileStates)stateNum);
	}

}
