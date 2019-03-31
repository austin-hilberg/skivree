using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	string turnAxis;
	string brakeAxis;
	string tuckAxis;

	Player player;

	// Use this for initialization
	void Start () {
		turnAxis = "Left Thumb X";
		brakeAxis = "Left Trigger";
		tuckAxis = "Right Trigger";

		player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		player.TurnSkis(Input.GetAxis(turnAxis));
		player.ApplyBrake(Input.GetAxis(brakeAxis));
		player.Tuck(Input.GetAxis(tuckAxis));
	}
}
