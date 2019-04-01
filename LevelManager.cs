using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	float slope;

	public GameObject tree;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateLevel (float slope) {
		this.slope = slope;

		AddIntervalObject (tree, 25, 0f, 0f, 10f, 10f);
		tree.SetActive(false);
	}

	void AddIntervalObject (GameObject gob, int n, float x, float y, float zStart, float zInterval) {
		Vector3 position = Vector3.zero;
		float angle = slope * Mathf.Deg2Rad;
		float sinAngle = Mathf.Sin(angle);
		float cosAngle = Mathf.Cos(angle);
		for (int i = 0; i < n; i ++) {
			float zFlat = zStart + (zInterval * i);
			position.Set(x, - zFlat * sinAngle, zFlat * cosAngle);
			Instantiate(gob, position, Quaternion.Euler(0f, 0f, 0f));
		}
	}
}
