using UnityEngine;
using System;
using System.Collections;

public class Tile : MonoBehaviour {

	public Sprite tileDefault;
	public Sprite tileObstacle;
	public Sprite tileWithItem;

	public bool isOccupied;
	public bool hasItem;

	public bool isObstacle;

	SpriteRenderer tileRenderer;

	void Awake() {
		tileRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update() {
		if (hasItem) {
			tileRenderer.sprite = tileWithItem;
		} else {
			if (isObstacle) {
				isOccupied = true;
				tileRenderer.sprite = tileObstacle;
			} else {
				tileRenderer.sprite = tileDefault;
			}
		}
	}

	public void SpawnItem() {
		hasItem = true;
	}

	public void PickItemFromTile() {
		hasItem = false;
	}

	public void LeaveTile() {
		isOccupied = false;
	}

	public void EnterTile() {
		isOccupied = true;
	}

}
