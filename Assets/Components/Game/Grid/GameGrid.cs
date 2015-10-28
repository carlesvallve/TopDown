using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class GameGrid : MonoBehaviour {

	public Transform container;

	public int width = 7;
	public int height = 9;
	public int tileWidth = 30;
	public int tileHeight = 25;

	private GameController game;
	private Vector3 swipeStart;

	public GridLayers layers;

	public TilePrefabs[] tilePrefabs;
	public Dictionary<TileTypes, GameObject> tiles = new Dictionary<TileTypes, GameObject>();

	public ObstaclePrefabs[] obstaclePrefabs;
	public Dictionary<ObstacleTypes, GameObject> obstacles = new Dictionary<ObstacleTypes, GameObject>();


	public void Init (GameController game) {
		this.game = game;

		// create tiles dictionary: tile prefabs will be accessible by type key
		for (int i = 0; i < tilePrefabs.Length; i++) {
			tiles.Add(tilePrefabs[i].type, tilePrefabs[i].prefab);
		}

		// create obstacles dictionary: obstacle prefabs will be accessible by type key
		for (int i = 0; i < obstaclePrefabs.Length; i++) {
			obstacles.Add(obstaclePrefabs[i].type, obstaclePrefabs[i].prefab);
		}

		InitializeGrid();
	}


	// ===============================================================
	// Create Grid
	// ===============================================================

	private void InitializeGrid () {
		// initialize grid layers
		layers = new GridLayers () {
			{ typeof(Tile), new GridLayer<Tile> (height, width) },
			{ typeof(Entity), new GridLayer<Entity> (height, width) },
		};

		// initialize grid tiles
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				Tile tile = CreateTile(TileTypes.WATER, x, y);
				SetTile(x, y, tile);
			}
		}
	}


	private Tile CreateTile (TileTypes type, int x, int y) {
		Transform parent = container.Find("Tiles");

		GameObject obj = (GameObject)Instantiate(tiles[type]);
		obj.transform.SetParent(parent, false);
		obj.name = "Tile " + x + " " + y;

		Tile tile = obj.GetComponent<Tile>();
		tile.Init(this, type, x, y);

		return tile;
	}


	public void ChangeTile (Tile tile, TileTypes type) {
		SetTile(tile.x, tile.y, CreateTile(type, tile.x, tile.y));
		Destroy(tile.gameObject);
	}


	public void SetTile (int x, int y, Tile tile) {
		layers.Set<Tile>(y, x, tile);
	}

	
	public void SetEntity (int x, int y, Entity entity) {
		layers.Set<Entity>(y, x, entity);
	}


	public Tile GetTile (int x, int y) {
		return layers.Get<Tile>(y, x);
	}


	public Tile GetTile (Vector3 pos) {
		float ratio = tileHeight / (float)tileWidth;

		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(- 0.4f + pos.y / ratio);

		return layers.Get<Tile>(y, x);
	}


	public Entity GetEntity (int x, int y) {
		return layers.Get<Entity>(y, x);
	}


	public Entity GetEntity (Vector3 pos) {
		float ratio = tileHeight / (float)tileWidth;

		int x = Mathf.RoundToInt(pos.x);
		int y = Mathf.RoundToInt(- 0.4f + pos.y / ratio);

		return layers.Get<Entity>(y, x);
	}


	// ===============================================================
	// Interaction
	// ===============================================================

	private bool isMouseDown = false;


	void Update () {
		// if we are in play mode, interaction will be handled by GameGrid
		if (game.tool != GameTools.PLAY || game.IsPaused()) {
			return;
		}

		OnMouseInteraction ();
	}


	public void OnMouseInteraction () {
		if (EventSystem.current.IsPointerOverGameObject()) {
			if (EventSystem.current.currentSelectedGameObject != null &&
				EventSystem.current.currentSelectedGameObject.GetComponent<Button>()) {
				return;
			}	
		}

		if (Input.GetMouseButtonDown(0)) {
			swipeStart = Input.mousePosition;
			isMouseDown = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
		}

		if (isMouseDown) {
			Vector3 vec = Input.mousePosition - swipeStart;
			
			if (vec.magnitude > 16) {
				Vector2 delta;
				if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y)) {
					delta = new Vector2(Mathf.Sign(vec.x), 0);
				} else {
					delta = new Vector2(0, Mathf.Sign(vec.y));
				}

				OnSwipe(delta);
				isMouseDown = false;
			}
		}
	}


	private void OnSwipe (Vector2 delta) {
		int dx = (int)delta.x;
		int dy = (int)delta.y;
		int x = game.player.x;
		int y = game.player.y;
		int steps = 0;

		Tile tile = GetTile(x, y);

		while (tile != null) {
			x += dx;
			y += dy;
			steps += 1;

			if (x < 0 || y < 0 || x > width - 1 || y > height - 1) {
				break;
			}

			tile = GetTile(x, y);
			if (tile == null) { break; }
			if (!tile.IsWalkable()) { break; }
		}

		x-= dx;
		y-= dy;
		steps -= 1;

		StartCoroutine(game.player.MoveToCoords(x, y, 0.15f * steps));
	}
}
