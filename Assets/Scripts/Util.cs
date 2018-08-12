﻿using UnityEngine;

public static class Util {
	public static int manhattanDistance(Coordinate c1, Coordinate c2) {
		return System.Math.Abs(c1.x - c2.x) + System.Math.Abs(c1.y - c2.y);
	}

	public static Vector3 getClosestVector3(Vector3 start, Vector3 up, Vector3 down, Vector3 left, Vector3 right) {
		float upd = Vector3.Distance(start, up);
		float downd = Vector3.Distance(start, down);
		float leftd = Vector3.Distance(start, left);
		float rightd = Vector3.Distance(start, right);

		// Debug.DrawLine(start, up, Color.magenta);
		// Debug.DrawLine(start, down, Color.magenta);
		// Debug.DrawLine(start, left, Color.magenta);
		// Debug.DrawLine(start, right, Color.magenta);

		if (upd < downd && upd < leftd && upd < rightd) {
			return up;
		} else if (downd < upd && downd < leftd && downd < rightd) {
			return down;
		} else if (leftd < downd && leftd < upd && leftd < rightd) {
			return left;
		} else if (rightd < downd && rightd < upd && rightd < leftd) {
			return right;
		} else if (upd <= downd && upd <= leftd && upd <= rightd) {
			return up;
		} else if (downd <= upd && downd <= leftd && downd <= rightd) {
			return down;
		} else if (leftd <= downd && leftd <= upd && leftd <= rightd) {
			return left;
		} else if (rightd <= downd && rightd <= upd && rightd <= leftd) {
			return right;
		} else {
			return Vector3.zero;
		}
	}
}
