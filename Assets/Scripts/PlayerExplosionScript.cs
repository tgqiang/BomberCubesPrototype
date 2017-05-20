using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionScript : MonoBehaviour {

	public GameObject[] leftExplosions;
	public GameObject[] rightExplosions;
	public GameObject[] upExplosions;
	public GameObject[] downExplosions;

	public float ATTACK_DURATION;
	float attackTimeRemaining;

	int upIndex;
	int downIndex;
	int leftIndex;
	int rightIndex;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (attackTimeRemaining <= 0) {
			CeaseAttackAfterTrigger ();
			attackTimeRemaining = 0;
		} else if (attackTimeRemaining > 0) {
			attackTimeRemaining -= Time.deltaTime;
		}
	}

	void CeaseAttackAfterTrigger() {
		if (upIndex != -1) {
			upExplosions [upIndex].SetActive (false);
		}

		if (downIndex != -1) {
			downExplosions [downIndex].SetActive (false);
		}

		if (leftIndex != -1) {
			leftExplosions [leftIndex].SetActive (false);
		}

		if (rightIndex != -1) {
			rightExplosions [rightIndex].SetActive (false);
		}
	}

	// This method uses respective arguments as indices to turn on certain explosions.
	// Therefore, to NOT turn on an explosion in a desired direction, the value "-1" must
	// be passed in to the argument representing explosions in that direction.
	//
	// 0: opens near explosions
	// 1: opens mid explosions
	// 2: opens far explosions
	public void TriggerExplosion(int upForce, int downForce, int leftForce, int rightForce) {
		upIndex = upForce;
		downIndex = downForce;
		leftIndex = leftForce;
		rightIndex = rightForce;

		for (int i = 0; i < 3; i++) {
			if (i == upForce) {			// turn on proper up explosion
				upExplosions [i].SetActive (true);
			} else {
				upExplosions [i].SetActive (false);
			}

			if (i == downForce) {		// turn on proper down explosion
				downExplosions [i].SetActive (true);
			} else {
				downExplosions [i].SetActive (false);
			}

			if (i == leftForce) {		// turn on proper left explosion
				leftExplosions [i].SetActive (true);
			} else {
				leftExplosions [i].SetActive (false);
			}

			if (i == rightForce) {		// turn on proper right explosion
				rightExplosions [i].SetActive (true);
			} else {
				rightExplosions [i].SetActive (false);
			}
		}

		attackTimeRemaining = ATTACK_DURATION;
	}
}
