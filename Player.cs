using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject cam;
	GameObject skis;
	public GameObject ground;

	Vector3 planeNorm = Vector3.up;
	float slope = 0f;
	float tanAngle;

	float g = 9.8f; // Meters per second squared.
	Vector3 gravityVector;
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

	bool crouching = false; // Prep for jump.
	float jumpBuildTime = 0f; // Amount jump has charged.
	float timeToFullJump = 0.5f; // Time in seconds after which jump is fullly charged.
	bool jumping = false;
	Vector3 jumpVec;
	float minJump = 2f; // Minimum jump speed.
	float crouchJumpBoost = 3f; // Max jump speed gained by crouching.
	float vertSpeed = 0f;
	float height = 0f; // Ground clearance.
	float dHeight; // Change in height.

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

	bool keepingScore = false;
	//ScoreTracker scoreTracker;
	LevelManager.ScoreMode scoreMode;
	LevelManager level;

	AudioSource crashSound;
	AudioSource rightGateSound;
	AudioSource wrongGateSound;
	AudioSource jumpSound;
	AudioSource landSound;

	// Use this for initialization
	void Start () {
		gravityVector = g * Vector3.down;
		skis = transform.Find("Skis").gameObject;
		skiAngleMaxLeft = 360f - skiAngleMaxRight;
		// scoreTracker = GetComponent<ScoreTracker>();
		level = GameObject.Find("Game").GetComponent<LevelManager>();

		Transform sounds = transform.Find("Sounds");
		crashSound = sounds.Find("Crash").GetComponent<AudioSource>();
		rightGateSound = sounds.Find("RightGate").GetComponent<AudioSource>();
		wrongGateSound = sounds.Find("WrongGate").GetComponent<AudioSource>();
		jumpSound = sounds.Find("Jump").GetComponent<AudioSource>();
		landSound = sounds.Find("Landing").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.Translate(transform.position - cam.transform.position, Space.World);		
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;

		speed = velocity.magnitude;

		ground.transform.Translate(transform.position.x - ground.transform.position.x, (transform.position.z * tanAngle * -1f) - ground.transform.position.y, transform.position.z - ground.transform.position.z, Space.World);
		

		if (crashing) {
			turnInput = 0f;
			brakeInput = 0.5f;
			crouching = false;
			jumpBuildTime = 0f;
			jumping = false;
			crashing = stopCoastingSpeed < speed;
		}

		if (crouching) {
			jumpBuildTime = Mathf.Clamp(jumpBuildTime + dt, 0f, timeToFullJump);
		}

		if (jumping) {
			jumpVec = Vector3.up * (minJump + crouchJumpBoost * jumpBuildTime / timeToFullJump);
			velocity += jumpVec;
			jumpBuildTime = 0f;
			inAir = true;
			jumping = false;
			jumpSound.Play();
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

		if (inAir) {
			velocity += gravityVector * dt;
			//height = transform.position.y + velocity.y * dt - ground.transform.position.y;
			height = transform.position.y + Vector3.Project(velocity * dt, planeNorm).y - ground.transform.position.y;
			if (height <= 0f) {
				transform.Translate(0f, ground.transform.position.y - transform.position.y, 0f);
				velocity -= Vector3.Project(velocity, planeNorm);
				inAir = false;
				landSound.Play();
			}
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

		if (keepingScore) {
			switch (scoreMode) {
				case LevelManager.ScoreMode.Slalom:
					keepingScore = level.UpdateProgress(transform.position);
				break;
				case LevelManager.ScoreMode.Tree:
					keepingScore = level.UpdateProgress(transform.position);
				break;
				default:
				break;
			}
		}
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

	public void ReadyJump() {
		if (!crouching && !crashing) {
			crouching = true;
			jumpBuildTime = 0f;
		}
	}

	public void ReleaseJump() {
		crouching = false;
		jumping = true;
	}

	public void ChangeSlope(float slope) {
		this.slope += slope;
		skis.transform.Rotate(slope, 0f, 0f, Space.World);
		rotQuat = Quaternion.AngleAxis(slope, Vector3.right);
		planeNorm = rotQuat * planeNorm;
		moveDirection = rotQuat * moveDirection;
		skiDirection = rotQuat * skiDirection;
		tanAngle = Mathf.Tan(slope * Mathf.Deg2Rad);
	}

	public void Crash() {
		crashing = true;
		crashSound.Play();
	}

	public void StartScoring(LevelManager.ScoreMode mode) {
		if (!keepingScore) {
			keepingScore = true;
			scoreMode = mode;
			level.StartScoring(mode, Time.time);
		}
	}

	public void StopScoring() {
		keepingScore = false;
	}

	public void GateSound(bool correct) {
		if (correct) {
			rightGateSound.Play();
		}
		else {
			wrongGateSound.Play();
		}
	}
}
