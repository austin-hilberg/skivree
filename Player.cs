using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject cam;
	GameObject skis;

	Vector3 planeNorm = new Vector3();
	float slope;

	Vector3 gravityVector = 9.8f * Vector3.down; // Gravitational acceleration in meters per second squared.
	Vector3 moveDirection = Vector3.forward;
	Vector3 skiDirection = Vector3.forward;
	Vector3 velocity;
	
	float speed = 0f;
	float hardMaxSpeed = 30f;
	float currentMaxSpeed = 0f;
	float drag = 1f;
	float maxDrag = 6.92964645563f; // Maximum drag at 45 degrees.
	
	float brakeInput = 0f;
	float brakeStrength = 0f;
	float maxBrake = 12.5f;
	float stopCoastingSpeed = 0.1f; // Speed to check for rounding velocity to zero.
	float stopCoastingBrake = 0.9f;
	float stopCoastingAngle = 89.5f;
	
	float tuck = 0f;
	float tuckInput = 0f;
	float tuckRate = 2f;
	float tuckDragReduction = 0.25f;

	bool inAir = false;
	bool crashing = true;

	float turnInput = 0f;
	float turnFraction = 0f;
	float skiAngle = 0f;
	float skiAngleMaxRight = 90f;
	float skiAngleMaxLeft;
	float maxSkiTurnRate = 120f; // in degrees per second.
	float maxVelocityTurnRate = 9f;
	float turnSpeedModifier = 0.95f;
	float brakeTuckModifier = 0.5f;
	Quaternion rotQuat;

	// Use this for initialization
	void Start () {
		skis = transform.Find("Skis").gameObject;
		skiAngleMaxLeft = 360f - skiAngleMaxRight;
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.Translate(transform.position - cam.transform.position, Space.World);		
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;

		speed = velocity.magnitude;

		if (crashing) {
			turnInput = 0f;
			brakeInput = 0.5f;
			crashing = stopCoastingSpeed < speed;
		}

		if (turnInput != 0f) {
			float dAngle = maxSkiTurnRate * turnInput * (1 + brakeTuckModifier * (brakeInput - tuckInput)) * dt;
			float targetSkiAngle = skiAngle + dAngle;
			if (skiAngleMaxRight < Mathf.Abs(targetSkiAngle)) {
				dAngle = 0f < dAngle ? skiAngleMaxRight - skiAngle : -skiAngleMaxRight - skiAngle;
			}
			rotQuat = Quaternion.AngleAxis(dAngle, planeNorm);
			skiDirection = rotQuat * skiDirection;
			skis.transform.Rotate(planeNorm, dAngle, Space.World);
			skiAngle += dAngle;
		}

		float dTuck = tuckInput - tuck;
		if (dTuck != 0f) {
			dTuck = tuckRate * dt * Mathf.Sign(dTuck);
			tuck += dTuck;
			tuck = tuck < 0f ? 0f : tuck;
			tuck = 1f < tuck ? 1f : tuck;
		}

		if (!inAir) {
			float turnFraction = Vector3.Angle(velocity, skiDirection) / skiAngleMaxRight;
			turnFraction *= turnFraction;
			brakeStrength = Mathf.Sqrt((brakeInput + turnFraction) / 2f) * maxBrake * dt;
			speed -= brakeStrength;
			speed *= 0f < speed ? 1f : 0f;
			float fractionMaxSpeed = speed / hardMaxSpeed;
			moveDirection = Vector3.RotateTowards(moveDirection, skiDirection, (1f - turnFraction * turnSpeedModifier) * maxVelocityTurnRate * dt * Mathf.Pow(1f - fractionMaxSpeed * turnSpeedModifier, 2f), 0f);
			velocity = moveDirection * speed;
			velocity += Vector3.Project(gravityVector * dt, moveDirection);
			drag = maxDrag * (1f - tuck * tuckDragReduction) * dt * fractionMaxSpeed; // Drag based on speed
			velocity -= velocity.normalized * drag;
			if (speed < stopCoastingSpeed && (stopCoastingBrake < brakeInput || stopCoastingAngle <= Vector3.Angle(skiDirection, Vector3.forward))) {
				velocity = Vector3.zero;
			}
		}

		transform.Translate(velocity * dt, Space.World);
	}

	public void TurnSkis(float amount) {
		turnInput = amount;
	}

	public void ApplyBrake(float amount) {
		brakeInput = amount;
	}

	public void Tuck(float amount) {
		tuckInput = amount;
	}

	public void ChangeSlope(float slope) {
		this.slope += slope;
		skis.transform.Rotate(slope, 0f, 0f, Space.World);
		rotQuat = Quaternion.AngleAxis(slope, Vector3.right);
		planeNorm = rotQuat * Vector3.up;
		moveDirection = rotQuat * moveDirection;
		skiDirection = rotQuat * skiDirection;
	}

	public void Crash() {
		crashing = true;
	}
}
