using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public delegate void MoveEndedHandler();
	public event MoveEndedHandler OnMoveEnded;

	public int x { get; set; }
	public int y { get; set; }
	protected bool moving = false;

	protected GameGrid grid;
	protected SpriteRenderer img;
	protected GameObject selector;


	public virtual void Init (GameGrid grid, int x, int y) {
		this.grid = grid;
		this.x = x;
		this.y = y;

		img = transform.Find("Sprite").GetComponent<SpriteRenderer>();

		grid.layers.Set<Entity>(y, x, this);

		selector = transform.Find("Selector").gameObject;
		selector.SetActive(false);

		LocateAtCoords(x, y);
	}


	public virtual void Select () {
		selector.SetActive(true);
	}


	public virtual void Deselect () {
		selector.SetActive(false);
	}


	public virtual bool IsSelected () {
		return selector.activeSelf;
	}


	public virtual void LocateAtCoords (int x, int y) {
		grid.SetEntity(this.x, this.y, null);

		this.x = x;
		this.y = y;

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		transform.localPosition = new Vector3(x, 0.4f + y * ratio, 0);

		img.sortingOrder = grid.height - y;

		grid.SetEntity(x, y, this);
	}


	public virtual IEnumerator MoveToCoords(int x, int y, float duration) {
		if (moving) { yield break; }
		
		moving = true;
		grid.SetEntity(this.x, this.y, null);

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		Vector3 startPos = new Vector3(this.x, 0.4f + this.y * ratio, 0);
		Vector3 endPos = new Vector3(x, 0.4f + y * ratio, 0);
		
		float t = 0f;
		while (t <= 1f) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			img.sortingOrder = grid.height - y;

			yield return null;
		}

		this.x = x;
		this.y = y;
		transform.localPosition = endPos;

		EndMove();
	}


	protected void EndMove () {
		moving = false;
		grid.SetEntity(x, y, this);

		if (OnMoveEnded != null) {
			OnMoveEnded.Invoke ();
		}
	}
}


