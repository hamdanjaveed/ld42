using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayManager : BlockManager, IBlockHandler {
	private List<Coordinate> currentPath = new List<Coordinate>();
	private List<List<Coordinate>> paths = new List<List<Coordinate>>();

	protected override void Start() {
		base.Start();
	}

	public void CompletePath() {
		if (currentPath.Count == 0) {
			// Debug.Log("No path to complete!");
		} else {
			paths.Add(currentPath);
			currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().confirmed = true);
			currentPath.Clear();
		}
	}

	public void CancelPath() {
		currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().state = SubwayBlock.State.EMPTY);
		currentPath.Clear();
	}

	public override void BlockClicked(GameObject go) {
		SubwayBlock block = go.GetComponent<SubwayBlock>();
		if (block.state != SubwayBlock.State.EMPTY) {
			// Can't start path here
			// Debug.Log("You can't do that!");
		} else {
			if (currentPath.Count == 0) {
				currentPath.Add(block.pos);
			} else {
				Block previousBlock = GetBlock(currentPath[currentPath.Count - 1]).GetComponent<Block>();
				if (block.isInLineWith(previousBlock)) {
					List<Coordinate> newPath = previousBlock.getPathToBlock(block);
					int ind = newPath.FindIndex(p => GetBlock(p).GetComponent<SubwayBlock>().state != SubwayBlock.State.EMPTY);
					if (ind != -1) {
						// Debug.Log("Path conflicts with previous path! " + ind);
					} else {
						newPath.ForEach(p => currentPath.Add(p));
					}
				} else {
					// Debug.Log("Path must be a straight line!");
				}
			}

			UpdatePath();
		}
	}

	public override void BlockHovered(GameObject go) {
		// Debug.Log("Block clicked in subway: " + go.GetComponent<Block>().pos);
	}

	private void UpdatePath() {
		// string s = "";
		// currentPath.ForEach(p => s += p + " ");
		// Debug.Log("Updating path with " + currentPath.Count + " blocks: " + s);
		if (currentPath.Count == 1) {
			GetBlock(currentPath[0]).GetComponent<SubwayBlock>().state = SubwayBlock.State.NODE;
		} else {
			// 0 or 2+ blocks
			for (int i = 0; i < currentPath.Count; i++) {
				Block block = GetBlock(currentPath[i]).GetComponent<Block>();
				SubwayBlock subwayBlock = GetBlock(currentPath[i]).GetComponent<SubwayBlock>();
				if (i == 0 || i == currentPath.Count - 1) {
					// Ends
					SubwayBlock.Direction dir;
					if (i == 0) {
						// Beginning
						Block nextBlock = GetBlock(currentPath[i + 1]).GetComponent<Block>();
						dir = block.getDirectionRelativeTo(nextBlock);
					} else {
						// Ending
						Block previousBlock = GetBlock(currentPath[i - 1]).GetComponent<Block>();
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
					Block previousBlock = GetBlock(currentPath[i - 1]).GetComponent<Block>();
					Block nextBlock = GetBlock(currentPath[i + 1]).GetComponent<Block>();
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
