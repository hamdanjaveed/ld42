using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Data/Block")]
public class BlockData : ScriptableObject {
	public int numBlocks;
	public int blockSizePx = 16;
	public int gapSizePx;
	public float PPU = 4.0f;

	public int totalBlockSizePx {
		get {
			return blockSizePx + gapSizePx;
		}
	}

	public float halfBlockSizePx {
		get {
			return totalBlockSizePx / 2.0f;
		}
	}

	public float totalBlockSizeUnit {
		get {
			return totalBlockSizePx / PPU;
		}
	}

	public float halfBlockSizeUnit {
		get {
			return halfBlockSizePx / PPU;
		}
	}

	public int totalSizePx {
		get {
			return totalBlockSizePx * numBlocks;
		}
	}

	public float totalSizeUnit {
		get {
			return totalSizePx / PPU;
		}
	}

	public float halfTotalSizeUnit {
		get {
			return totalSizeUnit / 2.0f;
		}
	}

	public float gapSizeUnit {
		get {
			return gapSizePx / PPU;
		}
	}

	public float halfGapSizeUnit {
		get {
			return gapSizeUnit / 2.0f;
		}
	}
}
