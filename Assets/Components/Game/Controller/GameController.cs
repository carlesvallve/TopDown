using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameController : MonoBehaviour {

	public GameTools tool;
	public GamePrefabs prefabs;
	
	public Player player { get; set; }
	public int maxItems { get; set; }
	public int maxStars { get; set; }

	private Hud hud;
	private GameGrid grid;
	private GameMap map;

	private List<Item> items;
	private List<Star> stars;

	private List<Collectable> collectables;
	private int moves = 0;

	private bool paused = false;

	
	public void Init () {
		

		grid = GetComponent<GameGrid>();
		grid.Init(this);

		hud = GetComponent<Hud>();
		hud.Init(grid);

		map = GetComponent<GameMap>();
		map.Init(this, grid, hud);

		collectables = new List<Collectable>();

		paused = false;
	}


	public void ResetGame () {
		moves = 0;
		collectables = new List<Collectable>();
		maxItems = GameObject.Find("Grid/Items").transform.childCount;
	}


	public void SetPlayerListeners () {
		// pick up collectable
		player.OnPickupCollectable += (Collectable collectable) => {
			collectables.Add(collectable);
			hud.AddCollectable(collectable);
			Destroy(collectable.gameObject);
			
		};

		// end moving
		player.OnMoveEnded += () => {
			moves += 1;
			hud.UpdateMoves(moves);

			if (maxItems > 0) {
				int itemCount = GetCollected(typeof(Item));
				if (itemCount == maxItems) {
					StartCoroutine(GameWin());
				}
			}
		};
	}


	private int GetCollected(Type type) {
		int i = 0;
		foreach (Collectable collectable in collectables) {
			if (collectable.GetType() == type) { i ++; }
		}
		return i;
	}


	public IEnumerator GameWin () {
		int starCount = GetCollected(typeof(Star));
		print ("YOU WIN (" + moves + " moves / " + starCount + " stars)");

		paused = true;

		yield return  StartCoroutine(hud.DisplayPopupWin(true, 0.5f));
		yield return new WaitForSeconds (2.0f);

		Application.LoadLevel("Game");
	}


	public void GameLose () {
		Application.LoadLevel("Game");
	}


	public bool IsPaused () {
		return paused;
	}

}
