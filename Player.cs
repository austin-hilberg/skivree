using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject cam;
	GameObject skis;

	Vector3 planeNorm = new Vector3();
	float slope;

	Vector3 moveDirection = Vector3.forward;
	Vector3 skiDirection = Vector3.forward;
	float speed = 5f;
	//float accelleration

	float turnInput = 0f;
	float skiAngle = 0f;
	float skiAngleMaxRight = 90f;
	float skiAngleMaxLeft;
	float maxSkiTurnRate = 120f; // in degrees per second.
	Quaternion rotQuat;

	// Use this for initialization
	void Start () {
		skis = transform.Find("Skis").gameObject;
		skiAngleMaxLeft = 360f - skiAngleMaxRight;
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.Translate(transform.position - cam.transform.position, Space.World);

		SkiTurn(Mathf.Cos(Time.time * 1.5f));
		
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;

		if (turnInput != 0f) {
			float dAngle = maxSkiTurnRate * turnInput * dt;
			float targetSkiAngle = skiAngle + dAngle;
			if (skiAngleMaxRight < Mathf.Abs(targetSkiAngle)) {
				dAngle = 0f < dAngle ? skiAngleMaxRight - skiAngle : -skiAngleMaxRight - skiAngle;
			}
			rotQuat = Quaternion.AngleAxis(dAngle, planeNorm);
			skiDirection = rotQuat * skiDirection;
			skis.transform.Rotate(planeNorm, dAngle, Space.World);
			skiAngle += dAngle;
		}

		transform.Translate(skiDirection * speed * dt, Space.World);
	}

	public void SkiTurn(float amount) {
		turnInput = amount;
		Debug.Log(turnInput);
	}

	public void ChangeSlope(float slope) {
		this.slope += slope;
		skis.transform.Rotate(slope, 0f, 0f, Space.World);
		rotQuat = Quaternion.AngleAxis(slope, Vector3.right);
		planeNorm = rotQuat * Vector3.up;
		moveDirection = rotQuat * moveDirection;
		skiDirection = rotQuat * skiDirection;
	}
}
