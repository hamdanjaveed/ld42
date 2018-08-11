using UnityEngine;

public class Block : MonoBehaviour {
	public Coordinate pos;
	public IBlockHandler blockHandler;

	private bool didMouseDown = false;

	void OnMouseOver() {
		blockHandler.BlockHovered(gameObject);
	}

	void OnMouseDown() {
		didMouseDown = true;
	}

	void OnMouseUp() {
		if (didMouseDown) {
			blockHandler.BlockClicked(gameObject);
			didMouseDown = false;
		}
	}

	void OnMouseExit() {
		didMouseDown = false;
	}
}
