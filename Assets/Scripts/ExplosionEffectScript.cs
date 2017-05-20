using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectScript : MonoBehaviour {

	public int explosionOwner;

	void OnTriggerEnter2D(Collider2D other) {
		Player player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			if (player.playerNum != explosionOwner && !player.hasOpenedAllSides) {
				player.Die ();
			}
		}
	}
}
