public struct Coordinate {
	public int x;
	public int y;

	public Coordinate(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override string ToString() {
		return "(" + x + ", " + y + ")";
	}

	public Coordinate getNormalizedDeltaTo(Coordinate c) {
		int dx = c.x - x;
		int dy = c.y - y;

		if (dx != 0) {
			return new Coordinate(System.Math.Sign(dx), 0);
		} else {
			return new Coordinate(0, System.Math.Sign(dy));
		}
	}

	public static Coordinate operator+(Coordinate c1, Coordinate c2) {
		return new Coordinate(c1.x + c2.x, c1.y + c2.y);
	}

	public static bool operator==(Coordinate c1, Coordinate c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator!=(Coordinate c1, Coordinate c2) {
		return !(c1 == c2);
	}
}
