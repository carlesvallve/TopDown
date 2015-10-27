using UnityEngine;
using System.Collections;

public class GameView : MonoBehaviour {

	private GameController controller;
	
	
	void Awake () {
		controller = GetComponent<GameController>();
		controller.Init();
	}
	
}
