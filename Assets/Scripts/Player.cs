using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : MonoBehaviour {

	public int playerNum;

	public bool hasItem;
	public float itemHeldTime;
	public float MAX_ITEM_HOLD_TIME;

	public bool isDead;
	public bool isGameOver;

	public int explodeOption;

	public SpriteRenderer playerRenderer;
	public Sprite[] explodeSprites;

	public MapManagerScript mapManager;

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
	public int maxMapSize;

	// Use this for initialization
	void Start () {
		// sets up player attributes
		hasItem = false;
		itemHeldTime = 0;
		explodeOption = -1;

		// assigns a reference of game map to player
		map = mapManager.map;
		maxMapSize = mapManager.mapSize - 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (!isGameOver) {
			// TODO:
			// 2. implement player explosion controls

			if (hasItem) {
				itemHeldTime += Time.deltaTime;
			}

			// if player has absorbed the item for the fixed set of time
			if (itemHeldTime >= MAX_ITEM_HOLD_TIME) {
				hasItem = false;
				mapManager.SpawnItemAtRandomTile ();
			}

			UpdateControllerForPlayer ();
		}
	}

	void UpdateControllerForPlayer() {
		var inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices [playerNum] : null;
		if (inputDevice == null) {
			//print ("Device not detected for player " + (playerNum + 1));
		} else {
			UpdateMovementWithInputDevice (inputDevice);
			UpdateExplosionWithInputDevice (inputDevice);
		}
	}

	void UpdateMovementWithInputDevice(InputDevice inputDevice) {
		//print ("Updating player movement for player " + (playerNum + 1));
		if (inputDevice.DPadUp.WasPressed) {
			MoveUp ();
		} else if (inputDevice.DPadDown.WasPressed) {
			MoveDown ();
		} else if (inputDevice.DPadLeft.WasPressed) {
			MoveLeft ();
		} else if (inputDevice.DPadRight.WasPressed) {
			MoveRight ();
		}
	}

	void UpdateExplosionWithInputDevice(InputDevice inputDevice) {
		//print ("Updating explosion for player " + (playerNum + 1));
		// UP: 4
		// DOWN: 1
		// LEFT: 3
		// RIGHT: 2
		if (inputDevice.RightTrigger.WasPressed) {		// ALL #10
			// TODO: explode in all 4 directions
			// (1 tile range each direction)
		} else if (inputDevice.Action4.WasPressed &&
				   inputDevice.Action3.WasPressed) {	// UP + LEFT #6
			// TODO: explode up and left
			// (2 tile range each direction)
		} else if (inputDevice.Action4.WasPressed &&
				   inputDevice.Action1.WasPressed) {	// UP + DOWN #5
			// TODO: explode up and down
			// (2 tile range each direction)
		} else if (inputDevice.Action4.WasPressed &&
				   inputDevice.Action2.WasPressed) {	// UP + RIGHT #4
			// TODO: explode up and right
			// (2 tile range each direction)
		} else if (inputDevice.Action3.WasPressed &&
				   inputDevice.Action1.WasPressed) {	// LEFT + DOWN #9
			// TODO: explode left and down
			// (2 tile range each direction)
		} else if (inputDevice.Action3.WasPressed &&
				   inputDevice.Action2.WasPressed) {	// LEFT + RIGHT #8
			// TODO: explode left and right
			// (2 tile range each direction)
		} else if (inputDevice.Action1.WasPressed &&
				   inputDevice.Action2.WasPressed) {	// DOWN + RIGHT #7
			// TODO: explode left and down
			// (2 tile range each direction)
		} else if (inputDevice.Action1.IsPressed) {		// DOWN #2
			// TODO: explode down
			// (4 tile range each direction)
		} else if (inputDevice.Action2.WasPressed) {	// RIGHT #1
			// TODO: explode right
			// (4 tile range each direction)
		} else if (inputDevice.Action3.WasPressed) {	// LEFT #3
			// TODO: explode left
			// (4 tile range each direction)
		} else if (inputDevice.Action4.WasPressed) {	// UP #0
			// TODO: explode up
			// (4 tile range each direction)
		}
	}

	void PickItemFromTile() {
		map [x, y].PickItemFromTile (this);
	}

	// Movement uses joystick
	// If [x == y], prefer x

	void MoveUp() {
		if (y > 0) {
			y -= 1;
			transform.Translate (Vector3.up);
		}
	}

	void MoveDown() {
		if (y < maxMapSize) {
			y += 1;
			transform.Translate (Vector3.down);
		}
	}

	void MoveLeft() {
		if (x > 0) {
			x -= 1;
			transform.Translate (Vector3.left);
		}
	}

	void MoveRight() {
		if (x < maxMapSize) {
			x += 1;
			transform.Translate (Vector3.right);
		}
	}

	void TriggerExplosion(int option) {
		// TODO: implement player explosion controls
		// based on the type of explosion button pressed on Controller
	}

	void Die() {
		if (hasItem) {
			hasItem = false;
			// TODO: set player inactive maybe?

			// drops held item at current spot
			map [x, y].SpawnItem ();
		}
	}
}
