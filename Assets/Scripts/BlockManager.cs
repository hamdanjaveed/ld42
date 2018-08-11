using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockManager : MonoBehaviour, IBlockHandler {
	[SerializeField] protected BlockData data;

	private GameObject[][] blocks;

	protected virtual void Start () {
		Vector3 topLeftOffset = (Vector3.right + Vector3.down) * (data.PPU / 2.0f);
		float cityBlockOffset = (data.blockSizePx + data.gapSizePx) / data.PPU;

		GameObject blockContainer = new GameObject();
		blockContainer.name = "Blocks";
		blockContainer.transform.parent = transform;

		blocks = new GameObject[data.numBlocks][];
		for (int x = 0; x < data.numBlocks; x++) {
			blocks[x] = new GameObject[data.numBlocks];
			for (int y = 0; y < data.numBlocks; y++) {
				GameObject block = Instantiate(GetBlockPrefab(x, y), transform.position, transform.rotation) as GameObject;
				block.transform.parent = blockContainer.transform;
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

	protected GameObject GetBlock(int x, int y) {
		return blocks[x][y];
	}

	protected abstract GameObject GetBlockPrefab(int x, int y);

	public abstract void BlockClicked(GameObject go);
	public abstract void BlockHovered(GameObject go);
}
