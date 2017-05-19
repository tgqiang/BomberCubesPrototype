using UnityEngine;
using System;
using System.Collections;

public class Tile : MonoBehaviour {

	public Sprite tileDefault;
	public Sprite tileWithItem;

	public bool hasItem;

	SpriteRenderer tileRenderer;

	void Awake() {
		tileRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update() {
		if (hasItem) {
			tileRenderer.sprite = tileWithItem;
		} else {
			tileRenderer.sprite = tileDefault;
		}
	}

	public void SpawnItem() {
		hasItem = true;
	}

	public void PickItemFromTile(Player p) {
		hasItem = false;
		p.hasItem = true;
	}
}
