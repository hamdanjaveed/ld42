using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : BlockManager {
	private const float blockChosenTimeThreshold = 5f; // Seconds between choosing a city block

	private List<Coordinate> availableBlocks;

	private float timeSinceLastBlockChosen;

	protected override void Start() {
		base.Start();

		availableBlocks = new List<Coordinate>(data.numBlocks * data.numBlocks);
		for (int x = 0; x < data.numBlocks; x++) {
			for (int y = 0; y < data.numBlocks; y++) {
				availableBlocks.Add(new Coordinate(x, y));
			}
		}

		timeSinceLastBlockChosen = 0;
	}

	void Update() {
		if (availableBlocks.Count > 0) {
			if (timeSinceLastBlockChosen > blockChosenTimeThreshold) {
				ChoosePairOfBlocks();
				timeSinceLastBlockChosen = 0;
			}
		}

		timeSinceLastBlockChosen += Time.deltaTime;
	}

	public override void BlockClicked(GameObject go) {
		// Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
	}

	public override void BlockHovered(GameObject go) {
		// Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
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

			GetBlock(availableBlocks[r]).GetComponent<CityBlock>().Choose();
			GetBlock(availableBlocks[r2]).GetComponent<CityBlock>().Choose();

			availableBlocks.RemoveAt(r);
			availableBlocks.RemoveAt(r2);
		}
	}

	private int manhattanDistance(Coordinate c1, Coordinate c2) {
		return System.Math.Abs(c1.x - c2.x) + System.Math.Abs(c1.y - c2.y);
	}
}
