using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Hud : MonoBehaviour {

	public Transform container;
	public Transform menuContainer;
	public Transform infoContainer;
	public Transform popupsContainer;

	public HudPrefabs prefabs;

	private Dictionary<GameTools, Button> buttons = new Dictionary<GameTools, Button>();
	private GameObject popupWin;
	private Text movesTxt;


	public void Init () {
		InitPopups();
		InitButtons();
		InitMoves();
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
		foreach(KeyValuePair<GameTools,Button> btn in buttons) {
			btn.Value.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
		}

		if (tool != GameTools.NONE) {
			Button button = buttons[tool];
			button.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.65f);
		}
	}


	public void EnableGameTool (GameTools tool, bool value) {
		Button button = buttons[tool];
		button.interactable = value;

		float alpha = value ? 1 : 0.5f;
		button.transform.Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, alpha);
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
