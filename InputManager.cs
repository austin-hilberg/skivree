using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	string turnAxis;

	Player player;

	// Use this for initialization
	void Start () {
		turnAxis = "Turning";

		player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		float x = Input.GetAxis(turnAxis);
		player.SkiTurn(x);
	}
}
