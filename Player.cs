using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject cam;
	Vector3 planeNorm = new Vector3();
	GameObject skis;
	float speed = 5f;
	//float accell

	// Use this for initialization
	void Start () {
		skis = transform.Find("Skis").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.Translate(transform.position - cam.transform.position, Space.World);
		
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;
		transform.Translate(transform.forward * speed * dt, Space.World);
	}
}
