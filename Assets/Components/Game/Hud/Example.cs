using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Types1 {
	WATER = 0,
	GROUND = 1,
	SOLID = 2
}


public enum Types2 {
	BUSH = 0,
	TREE = 1,
	ROCK = 2
}


public class Example : MonoBehaviour {

	public Dictionary<Types1, GameObject> dict1 = new Dictionary<Types1, GameObject>();
	public Dictionary<Types2, GameObject> dict2 = new Dictionary<Types2, GameObject>();

	void Awake() {

		dict1.Add(Types1.WATER, new GameObject());
		dict1.Add(Types1.GROUND, new GameObject());
		dict1.Add(Types1.SOLID, new GameObject());

		dict2.Add(Types2.BUSH, new GameObject());
		dict2.Add(Types2.TREE, new GameObject());
		dict2.Add(Types2.ROCK, new GameObject());

		ExecuteGenericMethod<Types1>(dict1);
		ExecuteGenericMethod<Types2>(dict2);
	}


	private void ExecuteGenericMethod<T> (Dictionary<T, GameObject> dict) {

		int max = dict.Count;

		for (int i = 0; i < max; i++) {
			T myType = (T)(object)i;
			print ("Generic type >>> " + myType);

			GameObject obj = dict[myType];
			print ("gameobject: " + obj);
		}
	}

}
