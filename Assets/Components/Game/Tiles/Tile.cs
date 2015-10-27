using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public TileStates state;
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

		// set tile image
		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		img.sortingOrder = grid.height - y;
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
}
