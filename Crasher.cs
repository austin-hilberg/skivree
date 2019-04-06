using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crasher : MonoBehaviour {

	float lastCollisionTime = 0f;
	float collisionCooldown = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter (Collider other) {
		if (other.gameObject.name == "Player" && Time.time - lastCollisionTime > collisionCooldown) {
			other.gameObject.GetComponent<Player>().Crash();
			lastCollisionTime = Time.time;
			Debug.Log("Crash!");
		}
	}
}
