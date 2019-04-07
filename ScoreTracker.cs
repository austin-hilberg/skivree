using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour {

	public enum Mode {Slalom, Free, Tree};
	Mode currentMode;
	bool tracking = false;

	float startTime;

	int slalomGates;
	float slalomGateDistance;
	float xSlalom;
	float wSlalom;
	bool slalomStartsLeft;
	int gatesPassed = 0;
	int gatesMissed = 0;
	float missedGatePenalty = 15f;

	Player player;
	LevelManager level;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Player>();
		level = GameObject.Find("Game").GetComponent<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		if (tracking) {
			switch (currentMode) {
				case Mode.Slalom:
					
				break;
				default:
				break;
			}
		}
	}

	public void ModeStart(Mode mode) {
		tracking = true;
		currentMode = mode;
		startTime = Time.time;
		switch (currentMode) {
			case Mode.Slalom:
				slalomGates = level.SlalomGates();
				slalomGateDistance = level.SlalomZ();
				xSlalom = level.SlalomX();
				wSlalom = level.SlalomW();
				slalomStartsLeft = level.SlalomStartLeft();
			break;
			default:
			break;
		}
	}
}
