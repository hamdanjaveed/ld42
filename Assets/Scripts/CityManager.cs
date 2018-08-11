using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour {
	private const int citySize = 10;
	private const int cityBlockSize = 16;
	private const float tileSize = 4.0f;

	private const int roadSize = 4;

	private const float blockChosenTimeThreshold = 5.0f; // Seconds between choosing a city block

	[SerializeField] private GameObject cityBlockPrefab;


	private GameObject[][] cityBlocks;
	private bool[][] chosen;

	private float timeSinceLastBlockChosen;

	void Start () {
		int citySizePx = (citySize * cityBlockSize) + ((citySize - 1) * roadSize);
		Vector3 topLeftOffset = (Vector3.left + Vector3.up) * ((citySizePx / 2.0f / 4.0f) - (cityBlockSize / 2.0f / tileSize));
		float cityBlockOffset = (cityBlockSize + roadSize) / tileSize;

		cityBlocks = new GameObject[citySize][];
		for (int x = 0; x < citySize; x++) {
			cityBlocks[x] = new GameObject[citySize];
			for (int y = 0; y < citySize; y++) {
				GameObject cityBlock = Instantiate(cityBlockPrefab, transform.position, transform.rotation) as GameObject;
				cityBlock.transform.parent = transform;
				cityBlock.transform.position += topLeftOffset + Vector3.right * (cityBlockOffset * x) + Vector3.down * (cityBlockOffset * y);
				cityBlock.name = "City Block (" + x + ", " + y + ")";
				cityBlocks[x][y] = cityBlock;
			}
		}

		timeSinceLastBlockChosen = Time.time;
	}

	void Update() {
		if (Time.time - timeSinceLastBlockChosen > blockChosenTimeThreshold) {
			ChooseRandomBlock();
			timeSinceLastBlockChosen = Time.time;
		}
	}

	private void ChooseRandomBlock() {
		int tries = 0;
		int dist = 0;
		int cbx1, cby1;
		int cbx2, cby2;
		do {
			cbx1 = Random.Range(0, citySize);
			cby1 = Random.Range(0, citySize);
			cbx2 = Random.Range(0, citySize);
			cby2 = Random.Range(0, citySize);
			dist = manhattanDistance(cbx1, cby1, cbx2, cby2);
			if (dist == 0) continue; // Same block
			tries++;
		} while (tries < 5 && dist < 5);

		cityBlocks[cbx1][cby1].GetComponent<CityBlock>().Choose();
		cityBlocks[cbx2][cby2].GetComponent<CityBlock>().Choose();
	}

	private int manhattanDistance(int x1, int y1, int x2, int y2) {
		return System.Math.Abs(x1 - x2) + System.Math.Abs(y1 - y2);
	}
}
