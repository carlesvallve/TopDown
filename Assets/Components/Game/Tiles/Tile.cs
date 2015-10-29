using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public TileStates state;
	public bool breakable = false;

	public TileTypes type { get; set; }
	public int x { get; set; }
	public int y { get; set; }

	protected GameGrid grid;
	protected SpriteRenderer img;


	public virtual void Init (GameGrid grid, TileTypes type, int x, int y) {
		this.grid = grid;
		this.type = type;
		this.x = x;
		this.y = y;

		// locate tile
		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, y * ratio, 0);

		// set tile image
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		img.sortingOrder = grid.height - y;
	}


	public bool IsBreakable () {
		return breakable;
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


	public virtual void UpdateState () {
		if (state == TileStates.NORMAL) {
			grid.ChangeTile(this, TileTypes.BROKEN);
			return;
		}

		if (state == TileStates.BROKEN) {
			grid.ChangeTile(this, TileTypes.WATER);
			return;
		}
	}

}
