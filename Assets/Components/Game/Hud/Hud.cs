using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Hud : MonoBehaviour {

	private GameGrid grid;

	public HudContainers containers;
	public HudPrefabs prefabs;

	private Dictionary<GameTools, Button> buttons = new Dictionary<GameTools, Button>();
	private GameObject popupWin;
	private Text movesTxt;

	private GameTools selectedTool;
	public TileTypes selectedTileType { get; private set; }
	public ObstacleTypes selectedObstacleType { get; private set; }


	public void Init (GameGrid grid) {
		this.grid = grid;

		InitPopups();
		InitButtons();
		InitMoves();

		containers.menuVertical.gameObject.SetActive(false);
		selectedTileType = TileTypes.WATER;
	}


	public void Msg (string msg) {
		movesTxt.text = msg;
		StartCoroutine(EraseMsg());
	}

	private IEnumerator EraseMsg() {
		yield return new WaitForSeconds(1f);
		movesTxt.text = "";
	}


	// ==========================================
	// Buttons
	// ==========================================

	private void InitButtons () {
		buttons[GameTools.TILE] = containers.menu.Find("ButtonTile").GetComponent<Button>();
		buttons[GameTools.OBSTACLE] = containers.menu.Find("ButtonObstacle").GetComponent<Button>();
		buttons[GameTools.ITEM] = containers.menu.Find("ButtonItem").GetComponent<Button>();
		buttons[GameTools.STAR] = containers.menu.Find("ButtonStar").GetComponent<Button>();
		buttons[GameTools.PLAYER] = containers.menu.Find("ButtonPlayer").GetComponent<Button>();
		buttons[GameTools.PLAY] = containers.menu.Find("ButtonPlay").GetComponent<Button>();
	}


	public void SelectGameTool (GameTools tool) {
		if (tool == GameTools.NONE) {
			return;
		}	

		if (containers.menuVertical.gameObject.activeSelf && tool == selectedTool) {
			HideVerticalMenu();
			return;
		}

		foreach(KeyValuePair<GameTools,Button> btn in buttons) {
			btn.Value.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
		}

		Button button = buttons[tool];
		button.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);

		if (tool == selectedTool || containers.menuVertical.gameObject.activeSelf) { 
			if (tool == GameTools.TILE) {
				ShowVerticalMenu<TileTypes>(grid.tiles, buttons[tool]);
			} else if (tool == GameTools.OBSTACLE) {
				ShowVerticalMenu<ObstacleTypes>(grid.obstacles, buttons[tool]);
			} else {
				HideVerticalMenu();
			}
		} else {
			HideVerticalMenu();
		}

		selectedTool = tool;
	}


	public void EnableGameTool (GameTools tool, bool value) {
		Button button = buttons[tool];
		button.interactable = value;

		float alpha = value ? 1 : 0.5f;
		button.transform.Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, alpha);
	}


	// ==========================================
	// Vertical Menu
	// ==========================================

	private void ShowVerticalMenu<T>(Dictionary<T, GameObject> dict, Button button) {
		Transform container = containers.menuVertical.Find("Container");

		for (int i = 0; i < container.childCount; i++) {
			Destroy(container.GetChild(i).gameObject);
		}

		int max = dict.Count;

		for (int i = 0; i < max; i++) {
			T type = (T)(object)i;

			// create button prefab
			GameObject obj = (GameObject)Instantiate(prefabs.buttonPrefab);
			obj.transform.SetParent(container, false);
			obj.name = "Button";
			obj.transform.localPosition = new Vector3(0, -i * 152, 0);

			// set button image
			Image image = obj.transform.Find("Image").GetComponent<Image>();
			image.sprite = dict[type].transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite;

			// set button handler
			Button menuButton = obj.GetComponent<Button>();
			menuButton.onClick.AddListener(delegate {
				SelectType<T>(type);
				button.transform.Find("Image").GetComponent<Image>().sprite = image.sprite;
				HideVerticalMenu();
			});
		}


		containers.menuVertical.localPosition = new Vector3(
			button.transform.localPosition.x, 
			containers.menuVertical.localPosition.y, 
			0
		);

		RectTransform rect = container.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(rect.sizeDelta.x, max * 150);

		containers.menuVertical.gameObject.SetActive(true);
	}


	public void HideVerticalMenu() {
		containers.menuVertical.gameObject.SetActive(false);
	}


	private void SelectType<T>(T type) {
		if (type is TileTypes) {
			selectedTileType = (TileTypes)(object)type;
			return; 
		}

		if (type is ObstacleTypes) { 
			selectedObstacleType = (ObstacleTypes)(object)type;
			return; 
		}
	}


	// ==========================================
	// Player moves
	// ==========================================

	private void InitMoves () {
		movesTxt = containers.info.Find("Moves").GetComponent<Text>();
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

		Transform parent = containers.info.Find(name + "s");
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
		popupWin = containers.popups.Find("PopupWin").gameObject;
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
