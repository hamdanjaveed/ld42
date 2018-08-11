using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockManager : MonoBehaviour, IBlockHandler {
	[SerializeField] protected BlockData data;
	[SerializeField] private GameObject blockPrefab;

	private GameObject[][] blocks;

	// Use this for initialization
	protected virtual void Start () {
		int citySizePx = (data.numBlocks * data.blockSizePx) + ((data.numBlocks - 1) * data.gapSizePx);
		Vector3 topLeftOffset = (Vector3.right + Vector3.down) * (data.PPU / 2.0f);
		float cityBlockOffset = (data.blockSizePx + data.gapSizePx) / data.PPU;

		blocks = new GameObject[data.numBlocks][];
		for (int x = 0; x < data.numBlocks; x++) {
			blocks[x] = new GameObject[data.numBlocks];
			for (int y = 0; y < data.numBlocks; y++) {
				GameObject block = Instantiate(blockPrefab, transform.position, transform.rotation) as GameObject;
				block.transform.parent = transform;
				block.transform.position += topLeftOffset + Vector3.right * (cityBlockOffset * x) + Vector3.down * (cityBlockOffset * y);
				block.name = "Block (" + x + ", " + y + ")";

				Block blockBlock = block.GetComponent<Block>();
				blockBlock.pos = new Coordinate(x, y);
				blockBlock.blockHandler = this;

				blocks[x][y] = block;
			}
		}
	}

	protected GameObject GetBlock(Coordinate c) {
		return blocks[c.x][c.y];
	}

	public abstract void BlockClicked(GameObject go);
	public abstract void BlockHovered(GameObject go);
}
