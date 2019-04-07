using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	float slope;
	float radSlope;
	float sinAngle;
	float cosAngle;

	public GameObject tree;
	public GameObject tallTree;
	public GameObject gate;
	public GameObject mogulGroup;
	public GameObject slalomEntry;

	float minimumObstacleDistance = 6f;

	float zStartGates = 50f;
	float bufferWidth = 10f;
	float freeWidth = 50f;
	int slalomGates = 24;
	float slalomWidth = 25f;
	float slalomGateWidth = 15f;
	float slalomGateDistance = 20f;
	bool slalomStartLeft = true;
	float slalomGateSlopeDistance;
	float xSlalom;

	int treeSlalomGates = 39;

	List<Vector3> occupiedPositions;

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
		slalomGateSlopeDistance = slalomGateDistance * cosAngle;

		// Slalom setup:
		xSlalom = freeWidth / 2f + bufferWidth + slalomWidth / 2f;
		slalomEntry.transform.Translate(PosOnSlope(xSlalom, 0f, zStartGates), Space.World);
		ClearOccupied();
		AddIntervalObject (gate, Mathf.CeilToInt(slalomGates / 2f), xSlalom - slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance, slalomGateDistance * 2, false);
		AddIntervalObject (gate, slalomGates / 2, xSlalom + slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance * 2, slalomGateDistance * 2, true);
		float[] xMogulGroupBounds = new float[] {xSlalom - slalomWidth / 2f, xSlalom + slalomWidth / 2f};
		float[] zMogulGroupBounds = new float[] {zStartGates, zStartGates + slalomGateDistance * slalomGates};
		SingleRandomObjectArea (mogulGroup, xMogulGroupBounds, 0f, zMogulGroupBounds, new int[] {1, slalomGates}, new int[] {0, 3}, true, true, minimumObstacleDistance);
		List<GameObject> bufferObstacles = new List<GameObject>();
		bufferObstacles.Add(tree);
		bufferObstacles.Add(tallTree);
		bufferObstacles.Add(mogulGroup);
		float[] obProb = new float[] {0.25f, 0.25f, 0.75f};
		bool[] yRandomRotation = new bool[] {true, true, true};
		bool[] normalToPlane = new bool[] {false, false, true};
		float[] xRange = new float[] {-freeWidth / 2f, freeWidth / 2f};
		float[] zRange = new float[] {zStartGates, zStartGates + 200f};
		RandomObjectArea(bufferObstacles, obProb, xRange, 0f, zRange, new int[] {2, 8}, new int[] {2, 4}, yRandomRotation, normalToPlane, minimumObstacleDistance);

		//AddIntervalObject(tree, 60, -10f, 0f, 5f, 5f, false);

		tree.SetActive(false);
		tallTree.SetActive(false);
		gate.SetActive(false);
		mogulGroup.SetActive(false);
		ClearOccupied();
	}

	void AddIntervalObject (GameObject gob, int n, float x, float y, float zStart, float zInterval, bool flip) {
		for (int i = 0; i < n; i ++) {
			PlaceObject(gob, PosOnSlope(x, y, zStart + (zInterval * i)), 0f, 0f, flip ? 180f : 0f);
		}
	}

	void SingleRandomObjectArea (GameObject gob, float[] xRange, float yOffset, float[] zFlatRange, int[] tiles, int[] obPerTileRange, bool yRandomRotation, bool normalToPlane, float minDist) {
		float w = xRange[1] - xRange[0];
		float l = zFlatRange[1] - zFlatRange[0];
		float tileWidth = w / tiles[0];
		float tileLength = l / tiles[1];
		for (int i = 0; i < tiles[0]; i ++) {
			for (int j = 0; j < tiles[1]; j ++) {
				for (int n = 0; n < Random.Range(obPerTileRange[0], obPerTileRange[1] + 1); n ++) {
					float xStart = xRange[0] + tileWidth * i;
					float zStart = zFlatRange[0] + tileLength * j;
					Vector3 pos = RandomUnoccupiedPosition(xStart, xStart + tileWidth, yOffset, zStart, zStart + tileLength, minDist);
					float xRotation = normalToPlane ? slope : 0f;
					float yRotation = yRandomRotation ? Random.Range(0f, 360f) : 0f;
					PlaceObject(gob, pos, xRotation, yRotation, 0f);
				}
			}
		}
	}

	void RandomObjectArea (List<GameObject> obs,  float[] obProb, float[] xRange, float yOffset, float[] zFlatRange, int[] tiles, int[] obPerTileRange, bool[] yRandomRotation, bool[] normalToPlane, float minDist) {
		float w = xRange[1] - xRange[0];
		float l = zFlatRange[1] - zFlatRange[0];
		float tileWidth = w / tiles[0];
		float tileLength = l / tiles[1];
		for (int i = 0; i < tiles[0]; i ++) {
			for (int j = 0; j < tiles[1]; j ++) {
				for (int n = 0; n < Random.Range(obPerTileRange[0], obPerTileRange[1] + 1); n ++) {
					float ob = Random.Range(0f, 1f);
					for (int o = 0; o < obProb.Length; o ++) {
						if (ob <= obProb[o]) {
							float xStart = xRange[0] + tileWidth * i;
							float zStart = zFlatRange[0] + tileLength * j;
							Vector3 pos = RandomUnoccupiedPosition(xStart, xStart + tileWidth, yOffset, zStart, zStart + tileLength, minDist);
							float xRotation = normalToPlane[o] ? slope : 0f;
							float yRotation = yRandomRotation[o] ? Random.Range(0f, 360f) : 0f;
							PlaceObject(obs[o], pos, xRotation, yRotation, 0f);
							break;
						}
						else {
							ob -= obProb[o];
						}
					}
				}
			}
		}
	}

	void PlaceObject (GameObject gob, Vector3 pos, float xRotation, float yRotation, float zRotation) {
		GameObject newGob = Instantiate(gob, pos, Quaternion.Euler(0f, yRotation, 0f));
		newGob.transform.Rotate( 0f, 0f, zRotation, Space.World);
		newGob.transform.Rotate(xRotation, 0f, 0f, Space.World);
		occupiedPositions.Add(pos);
	}

	Vector3 PosOnSlope (float x, float y, float z) {
		return new Vector3(x, y - z * sinAngle, z * cosAngle);
	}

	void ClearOccupied () {
		occupiedPositions = new List<Vector3>();
	}

	bool CheckOccupied (Vector3 pos, float minDist) {
		foreach (Vector3 v in occupiedPositions) {
			if (Vector3.Distance(pos, v) < minDist) {
				return true;
			}
		}
		return false;
	}

	Vector3 RandomUnoccupiedPosition (float xMin, float xMax, float yOffset, float zMin, float zMax, float minDist) {
		Vector3 pos = PosOnSlope(Random.Range(xMin, xMax), yOffset, Random.Range(zMin, zMax));
		while (CheckOccupied(pos, minDist)) {
			pos = PosOnSlope(Random.Range(xMin, xMax), yOffset, Random.Range(zMin, zMax));
		}
		return pos;
	}

	public int SlalomGates() {
		return slalomGates;
	}

	public float SlalomX () {
		return xSlalom;
	}

	public float StartZ() {
		return zStartGates * cosAngle;
	}

	public float SlalomZ() {
		return slalomGateDistance * cosAngle;
	}

	public float SlalomW(){
		return slalomGateWidth;
	}

	public bool SlalomStartLeft() {
		return slalomStartLeft;
	}
}
