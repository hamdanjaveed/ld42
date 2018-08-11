using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Data/Block")]
public class BlockData : ScriptableObject {
	public int numBlocks;
	public int blockSizePx = 16;
	public int gapSizePx = 4;
	public float PPU = 4.0f;
}
