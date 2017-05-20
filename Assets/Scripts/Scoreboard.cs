using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {

	public Text p1DeathText;
	public Text p1ItemText;
	public Text p2DeathText;
	public Text p2ItemText;

	int p1DeathCount;
	int p1ItemCount;

	int p2DeathCount;
	int p2ItemCount;
	
	// Update is called once per frame
	void Update () {
		p1DeathText.text = "Death Count: " + p1DeathCount;
		p1ItemText.text = "Item Count: " + p1ItemCount;

		p2DeathText.text = "Death Count: " + p2DeathCount;
		p2ItemText.text = "Item Count: " + p2ItemCount;
	}

	public void UpdateP1Death() {
		p1DeathCount += 1;
	}

	public void UpdateP2Death() {
		p2DeathCount += 1;
	}

	public void UpdateP1Item() {
		p1ItemCount += 1;
	}

	public void UpdateP2Item() {
		p2ItemCount += 1;
	}
}
