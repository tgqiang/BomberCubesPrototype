using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

	public float GAME_DURATION;		// in seconds

	float timeElapsed;

	public Text gameOverText;

	Player[] players;

	// Use this for initialization
	void Start () {
		// Gets all players in the game
		players = new Player[2];
		players [0] = GameObject.Find ("Player 1").GetComponent<Player> ();
		players [1] = GameObject.Find ("Player 2").GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;

		// If game duration is up
		if (timeElapsed >= GAME_DURATION) {
			players [0].isGameOver = true;
			players [1].isGameOver = true;
			gameOverText.enabled = true;
		}
	}
}
