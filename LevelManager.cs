using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	float slope;

	public GameObject tree;
	public GameObject gate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateLevel (float slope) {
		this.slope = slope;

		AddIntervalObject (gate, 10, -2f, 0f, 20f, 30f, false);
		AddIntervalObject (gate, 10, 2f, 0f, 35f, 30f, true);
		gate.SetActive(false);
		AddIntervalObject(tree, 60, -10f, 0f, 5f, 5f, false);
		AddIntervalObject(tree, 60, 10f, 0f, 5f, 5f, false);
		tree.SetActive(false);

	}

	void AddIntervalObject (GameObject gob, int n, float x, float y, float zStart, float zInterval, bool flip) {
		Vector3 position = Vector3.zero;
		float angle = slope * Mathf.Deg2Rad;
		float sinAngle = Mathf.Sin(angle);
		float cosAngle = Mathf.Cos(angle);
		for (int i = 0; i < n; i ++) {
			float zFlat = zStart + (zInterval * i);
			position.Set(x, - zFlat * sinAngle, zFlat * cosAngle);
			Instantiate(gob, position, Quaternion.Euler(0f, 0f, flip ? 180f : 0f));
		}
	}
}
