using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject ground;
	public GameObject playerObject;
	Player player;
	LevelManager level;

	float slope = 20f;

	// Use this for initialization
	void Start () {
		player = playerObject.GetComponent<Player>();
		level = GetComponent<LevelManager>();
		ground.transform.Rotate(slope, 0f, 0f, Space.World);
		player.ChangeSlope(slope);
		level.GenerateLevel(slope);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
