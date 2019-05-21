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
	public GameObject deadTree;
	public GameObject gate;
	public GameObject mogulGroup;
	public GameObject slalomEntry;
	public GameObject treeSlalomEntry;
	public GameObject finishGate;

	float minObsDist = 6f;

	float zStartGates = 50f;
	float bufferWidth = 10f;
	float freeWidth = 50f;

	int slalomGates = 24;
	float slalomWidth = 25f;
	float slalomGateWidth = 15f;
	float slalomGateDistance = 20f;
	bool slalomStartLeft = false;
	float xSlalom;
	float penaltySlalom = 5f;

	float treeAndFreeLength;

	int treeSlalomGates = 39;
	float treeSlalomWidth = 25f;
	float treeSlalomGateWidth = 10f;
	float treeSlalomGateDistance = 25f;
	bool treeSlalomStartLeft = false;
	float xTreeSlalom;
	float penaltyTreeSlalom = 5f;

	Vector3 triggerDimensions = new Vector3(20f, 5f, 0.25f);

	List<Vector3> occupiedPositions;
	
	public enum ScoreMode {Slalom, Free, Tree};
	ScoreMode currentMode;
	float scoreStartTime;
	int gateProgress = 0;
	int failedGates = 0;

	Player player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateLevel (float slope) {
		this.slope = slope;
		radSlope = slope * Mathf.Deg2Rad;
		sinAngle = Mathf.Sin(radSlope);
		cosAngle = Mathf.Cos(radSlope);

		treeAndFreeLength = (treeSlalomGates + 1) * treeSlalomGateDistance;

		// Slalom setup:
		xSlalom = freeWidth / 2f + bufferWidth + slalomWidth / 2f;
		slalomEntry.transform.Translate(PosOnSlope(xSlalom, 0f, zStartGates), Space.World);
		BoxCollider slalomBox = slalomEntry.AddComponent<BoxCollider>();
		slalomBox.size = triggerDimensions;
		slalomBox.center = new Vector3 (slalomBox.center.x, slalomBox.center.y + triggerDimensions.y / 2f, slalomBox.center.z);
		slalomBox.isTrigger = true;
		StartingGate slalomGate = slalomEntry.AddComponent<StartingGate>();
		slalomGate.SetMode(ScoreMode.Slalom);
		Instantiate(finishGate, Vector3.zero, Quaternion.identity).transform.Translate(PosOnSlope(xSlalom, 0f, zStartGates + slalomGateDistance * (slalomGates + 1)), Space.World);
		ClearOccupied();

		AddIntervalObject (gate, Mathf.CeilToInt(slalomGates / 2f), xSlalom - slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance, slalomGateDistance * 2, false);
		AddIntervalObject (gate, slalomGates / 2, xSlalom + slalomGateWidth / 2f, 0f, zStartGates + slalomGateDistance * 2, slalomGateDistance * 2, true);
		float[] xMogulGroupBounds = new float[] {xSlalom - slalomWidth / 2f, xSlalom + slalomWidth / 2f};
		float[] zMogulGroupBounds = new float[] {zStartGates, zStartGates + slalomGateDistance * slalomGates};
		SingleRandomObjectArea (mogulGroup, xMogulGroupBounds, 0f, zMogulGroupBounds, new int[] {1, slalomGates}, new int[] {0, 3}, true, true, minObsDist);
		

		// Buffer setup:
		List<GameObject> bufferObstacles = new List<GameObject>();
		bufferObstacles.Add(tree);
		bufferObstacles.Add(tallTree);
		bufferObstacles.Add(mogulGroup);
		float[] obProb = new float[] {0.25f, 0.25f, 0.75f};
		bool[] yRandomRotation = new bool[] {true, true, true};
		bool[] normalToPlane = new bool[] {false, false, true};
		float[] xRange = new float[] {-freeWidth / 2f, freeWidth / 2f};
		float[] zRange = new float[] {zStartGates, zStartGates + 200f};
		RandomObjectArea(bufferObstacles, obProb, xRange, 0f, zRange, new int[] {2, 8}, new int[] {2, 4}, yRandomRotation, normalToPlane, minObsDist);

		// Tree slalom setup:
		xTreeSlalom = -(freeWidth / 2f + bufferWidth + treeSlalomWidth / 2f);
		treeSlalomEntry.transform.Translate(PosOnSlope(xTreeSlalom, 0f, zStartGates), Space.World);
		BoxCollider treeSlalomBox = treeSlalomEntry.AddComponent<BoxCollider>();
		treeSlalomBox.size = triggerDimensions;
		treeSlalomBox.center = new Vector3 (treeSlalomBox.center.x, treeSlalomBox.center.y + triggerDimensions.y / 2f, treeSlalomBox.center.z);
		treeSlalomBox.isTrigger = true;
		StartingGate treeSlalomGate = treeSlalomEntry.AddComponent<StartingGate>();
		treeSlalomGate.SetMode(ScoreMode.Tree);
		Instantiate(finishGate, Vector3.zero, Quaternion.identity).transform.Translate(PosOnSlope(xTreeSlalom, 0f, zStartGates + treeAndFreeLength), Space.World);

		AddIntervalObject (gate, Mathf.CeilToInt(treeSlalomGates / 2f), xTreeSlalom - treeSlalomGateWidth / 2f, 0f, zStartGates + treeSlalomGateDistance, treeSlalomGateDistance * 2, false);
		AddIntervalObject (gate, treeSlalomGates / 2, xTreeSlalom + treeSlalomGateWidth / 2f, 0f, zStartGates + treeSlalomGateDistance * 2, treeSlalomGateDistance * 2, true);
		List<GameObject> treesForSlalom = new List<GameObject>();
		treesForSlalom.Add(tree);
		treesForSlalom.Add(tallTree);
		treesForSlalom.Add(deadTree);
		float[] treesForSlalomProb = new float[] {0.2f, 0.6f, 0.2f};
		bool[] yTreesForSlalomRot = new bool[] {true, true, true};
		bool[] treesForSlalomNorm = new bool[] {false, false, false};
		float[] xTreesForSlalom = new float[] {xTreeSlalom - treeSlalomWidth / 2f, xTreeSlalom + treeSlalomWidth / 2f};
		float[] zTreesForSlalom = new float[] {zStartGates, zStartGates + treeAndFreeLength};

		RandomObjectArea(treesForSlalom, treesForSlalomProb, xTreesForSlalom, 0f, zTreesForSlalom, new int[] {2, treeSlalomGates}, new int[] {1, 3}, yTreesForSlalomRot, treesForSlalomNorm, minObsDist);

		tree.SetActive(false);
		tallTree.SetActive(false);
		deadTree.SetActive(false);
		gate.SetActive(false);
		mogulGroup.SetActive(false);
		finishGate.SetActive(false);
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

	public void StartScoring(ScoreMode mode, float startTime) {
		currentMode = mode;
		scoreStartTime = startTime;
	}

	public bool UpdateProgress(Vector3 pos) {
		switch (currentMode) {
			case ScoreMode.Slalom:
				if (pos.z >= (zStartGates + slalomGateDistance * (gateProgress + 1)) * cosAngle) {
					if (gateProgress < slalomGates) {
						bool gateOdd = gateProgress % 2 == 0;
						bool leftIsCorrect = slalomStartLeft ? !gateOdd : gateOdd;
						bool leftOfGate = pos.x <= xSlalom - slalomGateWidth / 2f + (slalomStartLeft ? (gateOdd ? 1 : 0) : (gateOdd ? 0 : 1)) * slalomGateWidth;
						if ((leftIsCorrect & leftOfGate) | (!leftIsCorrect & !leftOfGate)) {
							player.GateSound(true);
						}
						else {
							player.GateSound(false);
							failedGates++;
						}
						gateProgress++;
					}
					else {
						Debug.Log("Finish!");
						float scoreTime = Time.time - scoreStartTime + (penaltySlalom * failedGates);
						Debug.Log("Failed gates: " + failedGates);
						Debug.Log("Slalom time: " + scoreTime);
						return false;
					}
				}
			break;
			case ScoreMode.Tree:
				if (pos.z >= (zStartGates + treeSlalomGateDistance * (gateProgress + 1)) * cosAngle) {
					if (gateProgress < treeSlalomGates) {
						bool gateOdd = gateProgress % 2 == 0;
						bool leftIsCorrect = treeSlalomStartLeft ? !gateOdd : gateOdd;
						bool leftOfGate = pos.x <= xTreeSlalom - treeSlalomGateWidth / 2f + (treeSlalomStartLeft ? (gateOdd ? 1 : 0) : (gateOdd ? 0 : 1)) * treeSlalomGateWidth;
						if ((leftIsCorrect & leftOfGate) | (!leftIsCorrect & !leftOfGate)) {
							player.GateSound(true);
						}
						else {
							player.GateSound(false);
							failedGates++;
						}
						gateProgress++;
					}
					else {
						Debug.Log("Finish!");
						float scoreTime = Time.time - scoreStartTime + (penaltyTreeSlalom * failedGates);
						Debug.Log("Failed gates: " + failedGates);
						Debug.Log("Slalom time: " + scoreTime);
						return false;
					}
				}
			break;
			default:
			break;
		}
		return true;
	}	
}
