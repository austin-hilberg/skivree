using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject ground;
	public GameObject playerObject;

	float slope = 20f;

	// Use this for initialization
	void Start () {
		ground.transform.Rotate(slope, 0f, 0f, Space.World);
		playerObject.transform.Rotate(slope, 0f, 0f, Space.World);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
