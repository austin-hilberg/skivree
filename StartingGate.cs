using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGate : MonoBehaviour {

	LevelManager.ScoreMode mode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMode(LevelManager.ScoreMode newMode) {
		mode = newMode;
	}

	public void OnTriggerEnter (Collider other) {
		if (other.gameObject.name == "Player") {
			other.gameObject.GetComponent<Player>().StartScoring(mode);
		}
	}
}
