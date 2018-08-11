using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayManager : BlockManager, IBlockHandler {
	private bool isDrawingPath;

	protected override void Start() {
		base.Start();

		isDrawingPath = false;
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (isDrawingPath) {
				// End path
			} else {
				// Start path
				// SubwayBlock block = GetBlock()
			}
		}
	}

	public override void BlockClicked(GameObject go) {
		Debug.Log("Block clicked in subway: " + go.GetComponent<Block>().pos);
	}

	public override void BlockHovered(GameObject go) {
		// Debug.Log("Block clicked in subway: " + go.GetComponent<Block>().pos);
	}
}
