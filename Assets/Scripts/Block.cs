using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour {
	public enum Direction {
		SAME,
		UP,
		RIGHT,
		DOWN,
		LEFT,
	}

	public Coordinate pos;
	public IBlockHandler blockHandler;

	private bool didMouseDown = false;

	public Direction getDirectionRelativeTo(Block b) {
		int dx = b.pos.x - pos.x;
		int dy = b.pos.y - pos.y;
		if (dx == 0 && dy == 0) {
			return Direction.SAME;
		} else if (System.Math.Abs(dx) > System.Math.Abs(dy)) {
			if (System.Math.Sign(dx) > 0) {
				return Direction.RIGHT;
			} else {
				return Direction.LEFT;
			}
		} else {

			if (System.Math.Sign(dy) > 0) {
				return Direction.DOWN;
			} else {
				return Direction.UP;
			}
		}
	}

	public bool isInLineWith(Block b) {
		return (b.pos.x == pos.x) || (b.pos.y == pos.y);
	}

	public List<Coordinate> getPathToBlock(Block b) {
		List<Coordinate> path = new List<Coordinate>();
		Coordinate start = pos;
		Coordinate end = b.pos;
		if (start == end) return path;

		Coordinate delta = start.getNormalizedDeltaTo(end);

		do {
			start += delta;
			path.Add(start);
		} while (start != end);

		return path;
	}

	void OnMouseOver() {
		blockHandler.BlockHovered(gameObject);
	}

	void OnMouseDown() {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			didMouseDown = true;
		}
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
