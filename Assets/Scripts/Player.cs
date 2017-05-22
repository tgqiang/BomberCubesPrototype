using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : MonoBehaviour {

	public int playerNum;

	public Scoreboard scoreboard;

	// VARIABLES FOR CONTROLLING ITEM HOLDING
	public bool hasItem;
	public float itemHeldTime;
	public float MAX_ITEM_HOLD_TIME;

	float moveCd;

	public bool isDead;
	public bool isGameOver;

	public SpriteRenderer playerRenderer;
	public GameObject[] explodeSprites;

	public MapManagerScript mapManager;

	// ENERGY COST FOR USING EXPLOSION
	public int NEAR_EXPLOSION_COST;
	public int MID_EXPLOSION_COST;
	public int FAR_EXPLOSION_COST;

	public float NEAR_EXPLOSION_RECOVERY_TIME;
	public float MID_EXPLOSION_RECOVERY_TIME;
	public float FAR_EXPLOSION_RECOVERY_TIME;

	// TIME VARIABLE FOR DETERMINING WHEN CAN NEXT EXPLOSION BE USED
	float rechargeTimeLeft;

	// TIME VARIABLE FOR DETERMING ENERGY DEGENERATION
	public float MAX_INACTIVITY_DURATION;
	public float DEGEN_RATE;
	float stasisDuration;
	bool degen;
	float degenDuration;

	int originXCoord;
	int originYCoord;
	float originX;
	float originY;

	bool isLeftPressed;
	bool isRightPressed;
	bool isUpPressed;
	bool isDownPressed;
	int count;
	public float INVOKE_DELAY;
	float inputWindow;

	public bool hasOpenedAllSides;

	Rigidbody2D rb;
	PlayerExplosionScript playerExplosion;
	Energy playerEnergy;
	Tile[,] map;


	// Coordinate system for array-index tile access:
	// access is by [x,y]
	// 
	//  [+y]
	//  |
	//  |
	//  |
	//  |
	//  |
	//  |_______________ [+x]
	//  0

	public int x;
	public int y;
	int maxMapSize;

	// Use this for initialization
	void Start () {
		// sets up player attributes
		hasItem = false;
		itemHeldTime = 0;
		stasisDuration = 0;
		count = 0;
		inputWindow = 0;
		moveCd = 0;
		originX = transform.position.x;
		originY = transform.position.y;
		originXCoord = x;
		originYCoord = y;

		rb = GetComponent<Rigidbody2D> ();
		playerExplosion = GetComponent<PlayerExplosionScript> ();
		playerEnergy = GetComponent<Energy> ();

		// assigns a reference of game map to player
		map = mapManager.map;
		maxMapSize = mapManager.mapSize - 1;

		map [x, y].EnterTile ();
	}
	
	// Update is called once per frame
	void Update () {

		if (!isGameOver && !isDead) {
			float deltaTime = Time.deltaTime;

			if ((isLeftPressed || isRightPressed || isUpPressed || isDownPressed)) {
				inputWindow += deltaTime;
			}

			moveCd -= deltaTime;

			if (inputWindow > INVOKE_DELAY) {
				// TODO: invoke attack
				InvokeAttack();
			}

			if (moveCd <= 0) {
				moveCd = 0;
			}

			if (degen) {
				degenDuration += deltaTime;

				if (degenDuration >= DEGEN_RATE) {
					playerEnergy.Modify (-1);
					degenDuration -= DEGEN_RATE;
				}
			}

			stasisDuration += deltaTime;

			if (stasisDuration >= MAX_INACTIVITY_DURATION) {
				degen = true;
			} else if (stasisDuration < MAX_INACTIVITY_DURATION) {
				degen = false;
				degenDuration = 0;
			}

			if (hasItem) {
				itemHeldTime += deltaTime;
			}

			if (rechargeTimeLeft < 0) {
				hasOpenedAllSides = false;
				FoldBack ();
				rechargeTimeLeft = 0;
			} else if (rechargeTimeLeft > 0) {
				rechargeTimeLeft -= deltaTime;
			}

			// if player has absorbed the item for the fixed set of time
			if (itemHeldTime >= MAX_ITEM_HOLD_TIME && hasItem) {
				hasItem = false;
				itemHeldTime = 0;
				if (playerNum == 0) {
					scoreboard.UpdateP1Item ();
				} else if (playerNum == 1) {
					scoreboard.UpdateP2Item ();
				}
				mapManager.SpawnItemAtRandomTile ();
			}
		}
	}

	void FixedUpdate() {
		UpdateControllerForPlayer ();
	}

	void UpdateControllerForPlayer() {
		var inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (inputDevice == null) {
			// do nothing
		} else {
			UpdateMovementWithInputDevice (inputDevice);
			UpdateExplosionIntentWithInputDevice (inputDevice);
			//UpdateExplosionWithInputDevice (inputDevice);
		}
	}

	void UpdateMovementWithInputDevice(InputDevice inputDevice) {
		if (rechargeTimeLeft <= 0) {
			if (inputDevice.DPadUp.IsPressed && moveCd <= 0) {
				MoveUp ();
			} else if (inputDevice.DPadDown.IsPressed && moveCd <= 0) {
				MoveDown ();
			} else if (inputDevice.DPadLeft.IsPressed && moveCd <= 0) {
				MoveLeft ();
			} else if (inputDevice.DPadRight.IsPressed && moveCd <= 0) {
				MoveRight ();
			}
		}
	}

	// Checks if there are other players blocking the current player's sides
	// Specify the desired sides to check for by turning on the respective direction booleans
	bool isRelevantSidesOccupied (bool up, bool down, bool left, bool right) {
		bool isLeftSideOccupied = (x - 1 >= 0) ? map[x - 1, y].isOccupied : false;
		bool isRightSideOccupied = (x + 1 <= maxMapSize) ? map[x + 1, y].isOccupied : false;
		bool isUpSideOccupied = (y + 1 <= maxMapSize) ? map[x, y + 1].isOccupied : false;
		bool isDownSideOccupied = (y - 1 >= 0) ? map[x, y - 1].isOccupied : false;

		bool outcome = false;

		if (left) {
			outcome = outcome || isLeftSideOccupied;
		}
		if (right) {
			outcome = outcome || isRightSideOccupied;
		}
		if (up) {
			outcome = outcome || isUpSideOccupied;
		}
		if (down) {
			outcome = outcome || isDownSideOccupied;
		}

		return outcome;
	}

	void OpenSides(bool up, bool down, bool left, bool right) {
		if (up) {
			explodeSprites [0].SetActive (true);
		} else {
			explodeSprites [0].SetActive (false);
		}

		if (down) {
			explodeSprites [1].SetActive (true);
		} else {
			explodeSprites [1].SetActive (false);
		}

		if (left) {
			explodeSprites [2].SetActive (true);
		} else {
			explodeSprites [2].SetActive (false);
		}

		if (right) {
			explodeSprites [3].SetActive (true);
		} else {
			explodeSprites [3].SetActive (false);
		}
	}

	void FoldBack() {
		for (int i = 0; i < 4; i++) {
			explodeSprites [i].SetActive (false);
		}
	}

	void UpdateExplosionIntentWithInputDevice(InputDevice inputDevice) {
		// UP: 4
		// DOWN: 1
		// LEFT: 3
		// RIGHT: 2
		if (rechargeTimeLeft <= 0) {
			if (inputDevice.RightTrigger.WasPressed && !isRelevantSidesOccupied(true, true, true, true)) {		// ALL: immediately trigger it

				// explode in all 4 directions
				// (1 tile range each direction)
				if (playerEnergy.CurrentEnergy >= NEAR_EXPLOSION_COST) {	// if player has enough energy
					hasOpenedAllSides = true;
					playerEnergy.Modify (-NEAR_EXPLOSION_COST);
					OpenSides (true, true, true, true);
					playerExplosion.TriggerExplosion (0, 0, 0, 0);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + NEAR_EXPLOSION_RECOVERY_TIME;
				}

			}
			if (inputDevice.Action4.WasPressed) {	// UP
				if (count < 2) {
					print ("Up fire command registered.");
					isUpPressed = true;
					count += 1;
				}
			}
			if (inputDevice.Action1.WasPressed) {	// DOWN
				if (count < 2) {
					print ("Down fire command registered.");
					isDownPressed = true;
					count += 1;
				}
			}
			if (inputDevice.Action2.WasPressed) {	// RIGHT
				if (count < 2) {
					print ("Right fire command registered.");
					isRightPressed = true;
					count += 1;
				}
			}
			if (inputDevice.Action3.WasPressed) {	// LEFT
				if (count < 2) {
					print ("Left fire command registered.");
					isLeftPressed = true;
					count += 1;
				}
			}
		}
	}

	//void UpdateExplosionWithInputDevice(InputDevice inputDevice) {
	void UpdateExplosionWithInputDevice(InputDevice inputDevice) {
		// UP: 4
		// DOWN: 1
		// LEFT: 3
		// RIGHT: 2
		if (rechargeTimeLeft <= 0) {
			if (inputDevice.RightTrigger.WasPressed &&
				!isRelevantSidesOccupied(true, true, true, true)) {		// ALL #10
				
				// explode in all 4 directions
				// (1 tile range each direction)
				if (playerEnergy.CurrentEnergy >= NEAR_EXPLOSION_COST) {	// if player has enough energy
					hasOpenedAllSides = true;
					playerEnergy.Modify (-NEAR_EXPLOSION_COST);
					OpenSides (true, true, true, true);
					playerExplosion.TriggerExplosion (0, 0, 0, 0);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + NEAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action4.WasPressed &&
					   inputDevice.Action3.WasPressed &&
					   !isRelevantSidesOccupied(true, false, true, false)) {	// UP + LEFT #6

				// explode up and left
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, false, true, false);
					playerExplosion.TriggerExplosion (1, -1, 1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action4.WasPressed &&
					   inputDevice.Action1.WasPressed &&
					   !isRelevantSidesOccupied(true, true, false, false)) {	// UP + DOWN #5

				// explode up and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, true, false, false);
					playerExplosion.TriggerExplosion (1, 1, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action4.WasPressed &&
					   inputDevice.Action2.WasPressed &&
					   !isRelevantSidesOccupied(true, false, false, true)) {	// UP + RIGHT #4

				// explode up and right
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, false, false, true);
					playerExplosion.TriggerExplosion (1, -1, -1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action3.WasPressed &&
					   inputDevice.Action1.WasPressed &&
					   !isRelevantSidesOccupied(false, true, true, false)) {	// LEFT + DOWN #9

				// explode left and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, true, true, false);
					playerExplosion.TriggerExplosion (-1, 1, 1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action3.WasPressed &&
					   inputDevice.Action2.WasPressed &&
					   !isRelevantSidesOccupied(false, false, true, true)) {	// LEFT + RIGHT #8

				// explode left and right
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, false, true, true);
					playerExplosion.TriggerExplosion (-1, -1, 1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action1.WasPressed &&
					   inputDevice.Action2.WasPressed &&
					   !isRelevantSidesOccupied(false, true, false, true)) {	// DOWN + RIGHT #7

				// explode left and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, true, false, true);
					playerExplosion.TriggerExplosion (-1, 1, -1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action1.IsPressed &&
					   !isRelevantSidesOccupied(false, true, false, false)) {		// DOWN #2

				// explode down
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, true, false, false);
					playerExplosion.TriggerExplosion (-1, 2, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action2.WasPressed &&
					   !isRelevantSidesOccupied(false, false, false, true)) {	// RIGHT #1

				// explode right
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, false, false, true);
					playerExplosion.TriggerExplosion (-1, -1, -1, 2);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action3.WasPressed &&
					   !isRelevantSidesOccupied(false, false, true, false)) {	// LEFT #3

				// explode left
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, false, true, false);
					playerExplosion.TriggerExplosion (-1, -1, 2, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (inputDevice.Action4.WasPressed &&
					   !isRelevantSidesOccupied(true, false, false, false)) {	// UP #0

				// explode up
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (true, false, false, false);
					playerExplosion.TriggerExplosion (2, -1, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			}
		}
	}

	void InvokeAttack() {
		// UP: 4
		// DOWN: 1
		// LEFT: 3
		// RIGHT: 2
		if (rechargeTimeLeft <= 0) {
			if (isUpPressed && isLeftPressed &&
				!isRelevantSidesOccupied(true, false, true, false)) {	// UP + LEFT #6

				// explode up and left
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, false, true, false);
					playerExplosion.TriggerExplosion (1, -1, 1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isUpPressed && isDownPressed &&
				!isRelevantSidesOccupied(true, true, false, false)) {	// UP + DOWN #5

				// explode up and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, true, false, false);
					playerExplosion.TriggerExplosion (1, 1, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isUpPressed && isRightPressed &&
				!isRelevantSidesOccupied(true, false, false, true)) {	// UP + RIGHT #4

				// explode up and right
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (true, false, false, true);
					playerExplosion.TriggerExplosion (1, -1, -1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isLeftPressed && isDownPressed &&
				!isRelevantSidesOccupied(false, true, true, false)) {	// LEFT + DOWN #9

				// explode left and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, true, true, false);
					playerExplosion.TriggerExplosion (-1, 1, 1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isLeftPressed && isRightPressed &&
				!isRelevantSidesOccupied(false, false, true, true)) {	// LEFT + RIGHT #8

				// explode left and right
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, false, true, true);
					playerExplosion.TriggerExplosion (-1, -1, 1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isDownPressed && isRightPressed &&
				!isRelevantSidesOccupied(false, true, false, true)) {	// DOWN + RIGHT #7

				// explode left and down
				// (2 tile range each direction)
				if (playerEnergy.CurrentEnergy >= MID_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-MID_EXPLOSION_COST);
					OpenSides (false, true, false, true);
					playerExplosion.TriggerExplosion (-1, 1, -1, 1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + MID_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isDownPressed && count == 1 &&
				!isRelevantSidesOccupied(false, true, false, false)) {		// DOWN #2

				// explode down
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, true, false, false);
					playerExplosion.TriggerExplosion (-1, 2, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isRightPressed && count == 1 &&
				!isRelevantSidesOccupied(false, false, false, true)) {	// RIGHT #1

				// explode right
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, false, false, true);
					playerExplosion.TriggerExplosion (-1, -1, -1, 2);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isLeftPressed && count == 1 &&
				!isRelevantSidesOccupied(false, false, true, false)) {	// LEFT #3

				// explode left
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (false, false, true, false);
					playerExplosion.TriggerExplosion (-1, -1, 2, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			} else if (isUpPressed && count == 1 &&
				!isRelevantSidesOccupied(true, false, false, false)) {	// UP #0

				// explode up
				// (4 tile range each direction)
				if (playerEnergy.CurrentEnergy >= FAR_EXPLOSION_COST) {		// if player has enough energy
					hasOpenedAllSides = false;
					playerEnergy.Modify (-FAR_EXPLOSION_COST);
					OpenSides (true, false, false, false);
					playerExplosion.TriggerExplosion (2, -1, -1, -1);
					rechargeTimeLeft = playerExplosion.ATTACK_DURATION + FAR_EXPLOSION_RECOVERY_TIME;
				}

			}

			isUpPressed = false;
			isDownPressed = false;
			isLeftPressed = false;
			isRightPressed = false;
			count = 0;
			inputWindow = 0;
		}
	}

	void MoveUp() {
		if (y < maxMapSize && !map[x, y + 1].isOccupied) {
			map [x, y].LeaveTile ();
			y += 1;
			map [x, y].EnterTile ();
			if (map [x, y].hasItem) {
				map [x, y].PickItemFromTile ();
				hasItem = true;
			}
			//transform.Translate (Vector3.up);
			rb.MovePosition(transform.position + Vector3.up);
			playerEnergy.Modify (1);
			stasisDuration = 0;
			moveCd = 0.25f;
		}
	}

	void MoveDown() {
		if (y > 0 && !map[x, y - 1].isOccupied) {
			map [x, y].LeaveTile ();
			y -= 1;
			map [x, y].EnterTile ();
			if (map [x, y].hasItem) {
				map [x, y].PickItemFromTile ();
				hasItem = true;
			}
			//transform.Translate (Vector3.down);
			rb.MovePosition(transform.position + Vector3.down);
			playerEnergy.Modify (1);
			stasisDuration = 0;
			moveCd = 0.25f;
		}
	}

	void MoveLeft() {
		if (x > 0 && !map[x - 1, y].isOccupied) {
			map [x, y].LeaveTile ();
			x -= 1;
			map [x, y].EnterTile ();
			if (map [x, y].hasItem) {
				map [x, y].PickItemFromTile ();
				hasItem = true;
			}
			//transform.Translate (Vector3.left);
			rb.MovePosition(transform.position + Vector3.left);
			playerEnergy.Modify (1);
			stasisDuration = 0;
			moveCd = 0.25f;
		}
	}

	void MoveRight() {
		if (x < maxMapSize && !map[x + 1, y].isOccupied) {
			map [x, y].LeaveTile ();
			x += 1;
			map [x, y].EnterTile ();
			if (map [x, y].hasItem) {
				map [x, y].PickItemFromTile ();
				hasItem = true;
			}
			//transform.Translate (Vector3.right);
			rb.MovePosition(transform.position + Vector3.right);
			playerEnergy.Modify (1);
			stasisDuration = 0;
			moveCd = 0.25f;
		}
	}

	public void Die() {
		print ("Player " + (playerNum + 1) + " has died.");
		isDead = true;
		map [x, y].LeaveTile ();

		if (playerNum == 0) {
			scoreboard.UpdateP1Death ();
		} else if (playerNum == 1) {
			scoreboard.UpdateP2Death ();
		}

		if (hasItem) {
			hasItem = false;

			// drops held item at current spot
			map [x, y].SpawnItem ();
		}

		isDead = false;
		transform.position = new Vector3 (originX, originY, 0);
		x = originXCoord;
		y = originYCoord;
	}

	void OnCollisionEnter2D (Collision2D other) {
		Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D> ();
		if (otherRb != null) {
			otherRb.velocity = Vector2.zero;
		}
	}
}
