using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour {
	private const int citySize = 10;
	private const int cityBlockSize = 16;
	private const float tileSize = 4.0f;

	private const int roadSize = 4;

	[SerializeField] private GameObject cityBlockPrefab;

	private GameObject[][] cityBlocks;

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
	}
}
