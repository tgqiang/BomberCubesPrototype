using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerScript : MonoBehaviour {

	public int mapSize;				// MUST BE EVEN NUMBER

	public GameObject tile;			// TILE TEMPLATE
	public Transform mapParent;		// SINGLETON, reference transform for tiles
	public Tile[,] map;				// Map area is mapSize * mapSize

	// Use this for initialization
	// Sets up the play field
	void Awake () {
		// initialize 2D-array map
		map = new Tile[mapSize, mapSize];

		// filling 2D-array map with tile instances
		float startX = -((mapSize / 2.0f) - 0.5f);
		float startY = -((mapSize / 2.0f) - 0.5f);

		for (int r = 0; r < mapSize; r++) {
			for (int c = 0; c < mapSize; c++) {
				// create tile
				GameObject t = GameObject.Instantiate (tile);
				t.transform.SetParent (mapParent);
				t.transform.localPosition = new Vector3 (startX + r, startY + c, 0);

				// store Tile script in the 2D-array map
				map [r, c] = t.GetComponent<Tile> ();

				// show tile in scene
				tile.SetActive (true);
			}
		}

		map [0, 1].isObstacle = true;
		map [1, 1].isObstacle = true;
		map [2, 1].isObstacle = true;

		map [1, 7].isObstacle = true;
		map [1, 8].isObstacle = true;
		map [1, 9].isObstacle = true;

		map [7, 8].isObstacle = true;
		map [8, 8].isObstacle = true;
		map [9, 8].isObstacle = true;

		map [8, 0].isObstacle = true;
		map [8, 1].isObstacle = true;
		map [8, 2].isObstacle = true;

		SpawnItemAtRandomTile ();
	}

	public void SpawnItemAtRandomTile() {
		// get coordinates of random tile to spawn item at
		int x = Random.Range(0, mapSize);
		int y = Random.Range(0, mapSize);

		if (map [x, y].isObstacle) {
			SpawnItemAtRandomTile ();
		} else {
			map [x, y].SpawnItem ();
		}
	}
}
