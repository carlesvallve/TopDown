using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Entity {

	public delegate void PickupCollectableHandler(Collectable collectable);
	public event PickupCollectableHandler OnPickupCollectable;


	public override void Init (GameGrid grid, int x, int y) {
		base.Init(grid, x, y);
	}


	public override void LocateAtCoords (int x, int y) {
		base.LocateAtCoords(x, y);
		img.sortingOrder = grid.height - y + 1;
	}


	public override IEnumerator MoveToCoords(int x, int y, float duration) {
		if (moving) { yield break; }

		grid.SetEntity(this.x, this.y, null);
		moving = true;

		float ratio = grid.tileHeight / (float)grid.tileWidth;
		Vector3 startPos = new Vector3(this.x, 0.4f + this.y * ratio, 0);
		Vector3 endPos = new Vector3(x, 0.4f + y * ratio, 0);
		
		float t = 0f;
		while (t <= 1f) {
			t += Time.deltaTime / duration;
			transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
			img.sortingOrder = grid.height - y + 1;

			PickupCollectableAtPos(transform.localPosition);
			yield return null;
		}

		this.x = x;
		this.y = y;
		transform.localPosition = endPos;

		EndMove();
	}


	private void PickupCollectableAtPos (Vector3 pos) {
		Entity entity = grid.GetEntity(pos);
		if (entity == null) { return; }

		if (entity is Collectable) {
			if (OnPickupCollectable != null) {
				OnPickupCollectable.Invoke ((Collectable)entity);
			}
		}
	}
}
