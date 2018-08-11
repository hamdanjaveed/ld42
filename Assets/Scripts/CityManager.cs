using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Coordinate {
	public int x;
	public int y;

	public Coordinate(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override string ToString() {
		return "(" + x + ", " + y + ")";
	}
}

public class CityManager : MonoBehaviour {
	bool f = false;
	private const int citySize = 5;
	private const int cityBlockSize = 16;
	private const float tileSize = 4.0f;

	private const int roadSize = 4;

	private const float blockChosenTimeThreshold = 0.4f; // Seconds between choosing a city block

	[SerializeField] private GameObject cityBlockPrefab;

	private GameObject[][] cityBlocks;
	private List<Coordinate> availableBlocks = new List<Coordinate>(citySize * citySize);

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

				// Debug.Log(y + citySize * x);
				availableBlocks.Add(new Coordinate(x, y));
			}
		}
	Application.Quit();
		timeSinceLastBlockChosen = Time.time;
	}

	void Update() {
		if (availableBlocks.Count > 0) {
			if (Time.time - timeSinceLastBlockChosen > blockChosenTimeThreshold) {
				ChoosePairOfBlocks();
				timeSinceLastBlockChosen = Time.time;
			}
		}
	}

	private void ChoosePairOfBlocks() {
		int r = Random.Range(0, availableBlocks.Count - 1);
		List<Coordinate> possible = availableBlocks.FindAll(coord => {
			return manhattanDistance(availableBlocks[r], coord) > 3;
		});

		if (possible.Count == 0) {
			// Debug.Log("Could not choose with coord: " + availableBlocks[r]);
			availableBlocks.RemoveAt(r);
		} else {
			int r2 = Random.Range(0, possible.Count - 1);
			cityBlocks[availableBlocks[r].x][availableBlocks[r].y].GetComponent<CityBlock>().Choose();
			cityBlocks[availableBlocks[r2].x][availableBlocks[r2].y].GetComponent<CityBlock>().Choose();

			availableBlocks.RemoveAt(r);
			availableBlocks.RemoveAt(r2);
		}
	}

	private int manhattanDistance(Coordinate c1, Coordinate c2) {
		return System.Math.Abs(c1.x - c2.x) + System.Math.Abs(c1.y - c2.y);
	}
}
