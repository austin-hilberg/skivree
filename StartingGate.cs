using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGate : MonoBehaviour {

	public ScoreTracker.Mode mode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter (Collider other) {
		if (other.gameObject.name == "Player") {
			other.gameObject.GetComponent<ScoreTracker>().ModeStart(mode);
		}
	}
}
