using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {
	[SerializeField] protected BlockData data;
	[SerializeField] private GameObject blockPrefab;

	private GameObject[][] blocks;

	// Use this for initialization
	protected virtual void Start () {
		int citySizePx = (data.numBlocks * data.blockSizePx) + ((data.numBlocks - 1) * data.gapSizePx);
		Vector3 topLeftOffset = (Vector3.left + Vector3.up) * ((citySizePx / 2.0f / 4.0f) - (data.blockSizePx / 2.0f / data.PPU));
		float cityBlockOffset = (data.blockSizePx + data.gapSizePx) / data.PPU;

		blocks = new GameObject[data.numBlocks][];
		for (int x = 0; x < data.numBlocks; x++) {
			blocks[x] = new GameObject[data.numBlocks];
			for (int y = 0; y < data.numBlocks; y++) {
				GameObject block = Instantiate(blockPrefab, transform.position, transform.rotation) as GameObject;
				block.transform.parent = transform;
				block.transform.position += topLeftOffset + Vector3.right * (cityBlockOffset * x) + Vector3.down * (cityBlockOffset * y);
				block.name = "Block (" + x + ", " + y + ")";
				blocks[x][y] = block;
			}
		}
	}

	protected GameObject GetBlock(Coordinate c) {
		return blocks[c.x][c.y];
	}
}
