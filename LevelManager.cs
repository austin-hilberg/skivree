using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	float slope;
	float radSlope;
	float sinAngle;
	float cosAngle;

	public GameObject tree;
	public GameObject gate;

	float minimumObstacleDistance = 6f;

	float zStartGates = 50f;
	float bufferWidth = 10f;
	float freeWidth = 50f;
	int slalomGates = 24;
	float slalomWidth = 25f;
	float slalomGateWidth = 15f;
	float slalomGateDistance = 20f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateLevel (float slope) {
		this.slope = slope;
		radSlope = slope * Mathf.Deg2Rad;
		sinAngle = Mathf.Sin(radSlope);
		cosAngle = Mathf.Cos(radSlope);

		// Slalom setup:
		float xSlalom = freeWidth / 2f + bufferWidth + slalomWidth / 2f;
		AddIntervalObject (gate, slalomGates / 2, xSlalom - slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance, slalomGateDistance * 2, false);
		AddIntervalObject (gate, slalomGates / 2, xSlalom + slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance * 2, slalomGateDistance * 2, true);
		
		gate.SetActive(false);
		AddIntervalObject(tree, 60, -10f, 0f, 5f, 5f, false);
		AddIntervalObject(tree, 60, 10f, 0f, 5f, 5f, false);
		tree.SetActive(false);

	}

	void AddIntervalObject (GameObject gob, int n, float x, float y, float zStart, float zInterval, bool flip) {
		for (int i = 0; i < n; i ++) {
			PlaceObject(gob, x, y, zStart + (zInterval * i), 0f, 0f, flip ? 180f : 0f);
		}
	}

	void AddIntervalGroup (GameObject gob, int nGroups, int groupMin, int groupMax, float xMin, float xMax, float y, float zStart, float zInterval, bool yRandomRotation, bool normalToPlane) {
		Vector3 position = Vector3.zero;
		for (int i = 0; i < nGroups; i ++) {
			int nThisGroup = Random.Range(groupMin, groupMax + 1);
			for (int j = 0; j < nThisGroup; j ++) {
				float x = Random.Range(xMin, xMax);
				float z = zStart + (zInterval * (j - Random.Range(0f, 1f)));
				float xRotation = normalToPlane ? slope : 0f;
				float yRotation = yRandomRotation ? Random.Range(0f, 360f) : 0f;
				PlaceObject(gob, x, y, z, xRotation, yRotation, 0f);
			}
		}
	}

	void PlaceObject (GameObject gob, float x, float yOffset, float zFlat, float xRotation, float yRotation, float zRotation) {
		Vector3 position = new Vector3(x, yOffset - zFlat * sinAngle, zFlat * cosAngle);
		Instantiate(gob, position, Quaternion.Euler(xRotation, yRotation, zRotation));
	}

}
