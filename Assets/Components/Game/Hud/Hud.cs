using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Hud : MonoBehaviour {

	private GameGrid grid;

	public Transform container;
	public Transform menuContainer;
	public Transform infoContainer;
	public Transform popupsContainer;

	public Transform menuVertical;

	public HudPrefabs prefabs;

	private Dictionary<GameTools, Button> buttons = new Dictionary<GameTools, Button>();
	private GameObject popupWin;
	private Text movesTxt;

	public TileTypes selectedTileType { get; private set; }
	private GameTools selectedTool;


	public void Init (GameGrid grid) {
		this.grid = grid;

		InitPopups();
		InitButtons();
		InitMoves();

		menuVertical.gameObject.SetActive(false);
		selectedTileType = TileTypes.WATER;
	}


	// ==========================================
	// Buttons
	// ==========================================

	private void InitButtons () {
		buttons[GameTools.TILE] = menuContainer.Find("ButtonTile").GetComponent<Button>();
		buttons[GameTools.OBSTACLE] = menuContainer.Find("ButtonObstacle").GetComponent<Button>();
		buttons[GameTools.ITEM] = menuContainer.Find("ButtonItem").GetComponent<Button>();
		buttons[GameTools.STAR] = menuContainer.Find("ButtonStar").GetComponent<Button>();
		buttons[GameTools.PLAYER] = menuContainer.Find("ButtonPlayer").GetComponent<Button>();
		buttons[GameTools.PLAY] = menuContainer.Find("ButtonPlay").GetComponent<Button>();
	}


	public void SelectGameTool (GameTools tool) {
		if (tool == GameTools.NONE) {
			return;
		}	

		foreach(KeyValuePair<GameTools,Button> btn in buttons) {
			btn.Value.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
		}

		Button button = buttons[tool];
		button.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
		
		if (tool == GameTools.TILE && tool == selectedTool) {
			DisplayTileTypeMenu();
		} else {
			menuVertical.gameObject.SetActive(false);
		}

		selectedTool = tool;
	}


	public void EnableGameTool (GameTools tool, bool value) {
		Button button = buttons[tool];
		button.interactable = value;

		float alpha = value ? 1 : 0.5f;
		button.transform.Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, alpha);
	}



	private void DisplayTileTypeMenu () {
		if (menuVertical.gameObject.activeSelf) {
			menuVertical.gameObject.SetActive(false);
			return;
		}

		Transform container = menuVertical.Find("Container");

		for (int i = 0; i < container.childCount; i++) {
			Destroy(container.GetChild(i).gameObject);
		}

		int max = System.Enum.GetValues(typeof(TileTypes)).Length;

		for (int i = 0; i < max; i++) {
			// create button prefab
			GameObject obj = (GameObject)Instantiate(prefabs.buttonPrefab);
			obj.transform.SetParent(container, false);
			obj.name = "Button";
			obj.transform.localPosition = new Vector3(0, -i * 152, 0);

			// assign button image
			Image image = obj.transform.Find("Image").GetComponent<Image>();
			image.sprite = grid.tiles[(TileTypes)i].transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite;

			// assign button handler
			int num = i;
			Button button = obj.GetComponent<Button>();
			button.onClick.AddListener(delegate {
				// select tile type
				selectedTileType = (TileTypes)num;
				buttons[GameTools.TILE].transform.Find("Image").GetComponent<Image>().sprite = image.sprite;
				menuVertical.gameObject.SetActive(false); 
			});
		}

		RectTransform rect = container.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(rect.sizeDelta.x, max * 150);

		menuVertical.gameObject.SetActive(true);
	}


	// ==========================================
	// Player moves
	// ==========================================

	private void InitMoves () {
		movesTxt = infoContainer.Find("Moves").GetComponent<Text>();
	}

	public void UpdateMoves (int moves) {
		movesTxt.text = "Moves: " + moves;
	}

	// ==========================================
	// Collectables (Items & Stars)
	// ==========================================


	public void AddCollectable (Entity entity) {
		string name = null;
		GameObject prefab = null;

		if (entity is Item) {
			name = "Item";
			prefab = prefabs.itemPrefab;
		} else if (entity is Star) {
			name = "Star";
			prefab = prefabs.starPrefab;
		}

		Transform parent = infoContainer.Find(name + "s");
		GameObject obj = (GameObject)Instantiate(prefab);
		obj.transform.SetParent(parent, false);
		obj.name = name;

		RectTransform rect = parent.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(120 * parent.childCount, rect.sizeDelta.y);
	}


	// ==========================================
	// Popups
	// ==========================================

	private void InitPopups () {
		popupWin = popupsContainer.Find("PopupWin").gameObject;
		popupWin.SetActive(false);
	}


	public virtual IEnumerator DisplayPopupWin (bool value, float duration) {
		float start = value ? 0 : 1;
		float end = value ? 1 : 0;
		
		if (value) {
			popupWin.SetActive(true);
		}

		CanvasGroup group = popupWin.GetComponent<CanvasGroup>();
		group.alpha = start;

		float t = 0f;
		while (t <= 1f) {
			t += Time.deltaTime / duration;
			group.alpha = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}

		group.alpha = end;

		if (!value) {
			popupWin.SetActive(value);
		}
	}
}
