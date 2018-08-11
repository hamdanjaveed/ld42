using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayManager : BlockManager, IBlockHandler {
	private bool isDrawingPath;
	private Coordinate startPos;
	private List<Coordinate> path = new List<Coordinate>();

	protected override void Start() {
		base.Start();

		isDrawingPath = false;
	}

	public override void BlockClicked(GameObject go) {
		SubwayBlock block = go.GetComponent<SubwayBlock>();
		if (isDrawingPath) {
			// End path
			Coordinate endPos = block.pos;
		} else {
			// Start path
			if (block.state != SubwayBlock.State.EMPTY) {
				// Can't start path here
				// Debug.Log("You can't do that!");
			} else {
				if (path.Count == 0) {
					path.Add(block.pos);
				} else {
					Block previousBlock = GetBlock(path[path.Count - 1]).GetComponent<Block>();
					if (block.isInLineWith(previousBlock)) {
						List<Coordinate> newPath = previousBlock.getPathToBlock(block);
						int ind = newPath.FindIndex(p => GetBlock(p).GetComponent<SubwayBlock>().state != SubwayBlock.State.EMPTY);
						if (ind != -1) {
							// Debug.Log("Path conflicts with previous path! " + ind);
						} else {
							newPath.ForEach(p => path.Add(p));
						}
					} else {
						// Debug.Log("Path must be a straight line!");
					}
				}

				UpdatePath();
			}
		}
	}

	public override void BlockHovered(GameObject go) {
		// Debug.Log("Block clicked in subway: " + go.GetComponent<Block>().pos);
	}

	private void UpdatePath() {
		// string s = "";
		// path.ForEach(p => s += p + " ");
		// Debug.Log("Updating path with " + path.Count + " blocks: " + s);
		if (path.Count == 1) {
			GetBlock(path[0]).GetComponent<SubwayBlock>().state = SubwayBlock.State.NODE;
		} else {
			// 0 or 2+ blocks
			for (int i = 0; i < path.Count; i++) {
				Block block = GetBlock(path[i]).GetComponent<Block>();
				SubwayBlock subwayBlock = GetBlock(path[i]).GetComponent<SubwayBlock>();
				if (i == 0 || i == path.Count - 1) {
					// Ends
					SubwayBlock.Direction dir;
					if (i == 0) {
						// Beginning
						Block nextBlock = GetBlock(path[i + 1]).GetComponent<Block>();
						dir = block.getDirectionRelativeTo(nextBlock);
					} else {
						// Ending
						Block previousBlock = GetBlock(path[i - 1]).GetComponent<Block>();
						dir = block.getDirectionRelativeTo(previousBlock);
					}

					switch (dir) {
						case SubwayBlock.Direction.UP:
							subwayBlock.state = SubwayBlock.State.NODE_UP;
							break;
						case SubwayBlock.Direction.RIGHT:
							subwayBlock.state = SubwayBlock.State.NODE_RIGHT;
							break;
						case SubwayBlock.Direction.LEFT:
							subwayBlock.state = SubwayBlock.State.NODE_LEFT;
							break;
						case SubwayBlock.Direction.DOWN:
							subwayBlock.state = SubwayBlock.State.NODE_DOWN;
							break;
					}
				} else {
					// Middle
					Block previousBlock = GetBlock(path[i - 1]).GetComponent<Block>();
					Block nextBlock = GetBlock(path[i + 1]).GetComponent<Block>();
					SubwayBlock.Direction previousDir = block.getDirectionRelativeTo(previousBlock);
					SubwayBlock.Direction nextDir = block.getDirectionRelativeTo(nextBlock);

					// Horizontal
					if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.RIGHT) || (previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.LEFT)) {
						subwayBlock.state = SubwayBlock.State.HORIZONTAL;
					} else if ((previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.UP)) {
						subwayBlock.state = SubwayBlock.State.VERTICAL;
					} else if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.LEFT)) {
						subwayBlock.state = SubwayBlock.State.CORNER_DOWN_LEFT;
					} else if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.UP) || (previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.LEFT)) {
						subwayBlock.state = SubwayBlock.State.CORNER_LEFT_UP;
					} else if ((previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.RIGHT)) {
						subwayBlock.state = SubwayBlock.State.CORNER_RIGHT_DOWN;
					} else if ((previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.UP) || (previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.RIGHT)) {
						subwayBlock.state = SubwayBlock.State.CORNER_UP_RIGHT;
					} else {
						Debug.Log("OOPS!");
					}
				}
			}
		}
	}
}
