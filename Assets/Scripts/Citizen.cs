using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Path {
	private List<PathSegment> segments;
	public float weight;

	public Path(List<PathSegment> segments) {
		this.segments = segments;

		if (segments.Count == 0) {
			weight = float.MaxValue;
		} else {
			float sum = 0;
			segments.ForEach(ps => sum += ps.weight);
			weight = sum;
		}
	}

	public void AddSegment(PathSegment segment) {
		segments.Add(segment);
		weight += segment.weight;
	}

	public static Path Empty() {
		return new Path(new List<PathSegment>());
	}

	public bool IsEmpty() {
		return segments.Count == 0;
	}

	public Vector3 NextDestination() {
		return segments[0].end;
	}

	public Vector3 DestinationAt(int i) {
		return segments[i].end;
	}

	public int Count() {
		return segments.Count;
	}

	public void RemoveFirstSegment() {
		segments.RemoveAt(0);
	}

	public static bool operator<(Path p1, Path p2) {
		return p1.weight < p2.weight;
	}

	public static bool operator>(Path p1, Path p2) {
		return p1.weight > p2.weight;
	}

	public override string ToString() {
		return "Path (" + weight + ")";
	}
}

struct PathSegment {
	public Vector3 begin;
	public Vector3 end;
	public float weight;

	public PathSegment(Vector3 begin, Vector3 end, float weight) {
		this.begin = begin;
		this.end = end;
		this.weight = weight;
	}

	public PathSegment(Vector3 begin, Vector3 end) {
		this.begin = begin;
		this.end = end;
		this.weight = Util.manhattanWeight(begin, end);
	}
}

public class Citizen : MonoBehaviour {
	private enum State {
		MOVING_IN,
		AT_HOME,
		GOING_TO_WORK,
		AT_WORK,
		GOING_HOME,
	}

	private State state = State.MOVING_IN;

	private SpriteRenderer spriteR;
	private string citizenName;

	private ResidentialCityBlock home;
	private Vector3 idlePos;
	private Path walkingPath;

	private IndustrialCityBlock work;

	private float moveSpeed;
	private float stateTimer = 0;
	private float idleTimer = 0;

	private static Color[] skinTones = {
		new Color(0.553f, 0.333f, 0.141f),
		new Color(0.776f, 0.525f, 0.259f),
		new Color(0.878f, 0.675f, 0.412f),
		new Color(0.945f, 0.761f, 0.490f),
		new Color(1.000f, 0.859f, 0.675f),
	};
	private static Color[] pantsColors = {
		new Color(0.102f, 0.737f, 0.612f),
		new Color(0.180f, 0.800f, 0.443f),
		new Color(0.086f, 0.627f, 0.522f),
		new Color(0.153f, 0.682f, 0.376f),
		new Color(0.204f, 0.596f, 0.859f),
		new Color(0.161f, 0.502f, 0.725f),
		new Color(0.608f, 0.349f, 0.714f),
		new Color(0.557f, 0.267f, 0.678f),
		new Color(0.204f, 0.286f, 0.369f),
		new Color(0.173f, 0.243f, 0.314f),
		new Color(0.945f, 0.769f, 0.059f),
		new Color(0.953f, 0.612f, 0.071f),
		new Color(0.902f, 0.494f, 0.133f),
		new Color(0.827f, 0.329f, 0.000f),
		new Color(0.906f, 0.298f, 0.235f),
		new Color(0.753f, 0.224f, 0.169f),
		new Color(0.925f, 0.941f, 0.945f),
		new Color(0.741f, 0.765f, 0.78f),
		new Color(0.584f, 0.647f, 0.651f),
		new Color(0.498f, 0.549f, 0.553f),
	};

	void Start() {
		citizenName = GenerateName();
		gameObject.name = citizenName;

		moveSpeed = Random.Range(1.25f, 1.75f);

		Texture2D tex = new Texture2D(1, 2, TextureFormat.RGB24, false);
		tex.SetPixel(0, 0, pantsColors[Random.Range(0, pantsColors.Length - 1)]);
		tex.SetPixel(0, 1, skinTones[Random.Range(0, skinTones.Length - 1)]);
		tex.filterMode = FilterMode.Point;
		tex.Apply();

		Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4f);

		spriteR = GetComponent<SpriteRenderer>();
		spriteR.sprite = s;
	}

	void Update() {
		stateTimer += Time.deltaTime;
		idleTimer += Time.deltaTime;

		switch (state) {
			case State.MOVING_IN:
				WalkAlongPath(home.pos, home.transform.position, () => {
					state = State.AT_HOME;
					BeginIdlingIn(home.pos);
					ResetTimers();
				}, 5.0f);
				break;
			case State.AT_HOME:
				IdleIn(home.pos);
				break;
			case State.GOING_TO_WORK:
				WalkAlongPath(work.pos, work.transform.position, () => {
					state = State.AT_WORK;
					BeginIdlingIn(work.pos);
					ResetTimers();
				});
				break;
			case State.AT_WORK:
				IdleIn(work.pos);
				break;
			case State.GOING_HOME:
				WalkAlongPath(home.pos, home.transform.position, () => {
					state = State.AT_HOME;
					ResetTimers();
					BeginIdlingIn(home.pos);
				});
				break;
		}
	}

	public void GoToWork() {
		if (state == State.AT_HOME) {
			state = State.GOING_TO_WORK;
			ResetTimers();
			BeginWalk(work.pos.GetPos());
		}
	}

	public void GoHome() {
		if (state == State.AT_WORK || state == State.GOING_TO_WORK) {
			state = State.GOING_HOME;
			ResetTimers();
			BeginWalk(home.pos.GetPos());
		}
	}

	public void SetSpawn(Vector3 spawn) {
		transform.position = spawn;
		BeginWalk(home.pos.GetPos());
	}

	public void SetHome(ResidentialCityBlock home) {
		this.home = home;
	}

	public void SetWork(IndustrialCityBlock work) {
		this.work = work;
	}

	public string GetName() {
		return citizenName;
	}

	public override string ToString() {
		return GetName();
	}

	private void ResetTimers() {
		stateTimer = 0;
		idleTimer = 2;
	}

	private void BeginWalk(Vector3 dest) {
		walkingPath = GetManhattanPath(transform.position, dest);
	}

	private Path GetBestPath(Vector3 start, Coordinate end) {
		Path p = new Path();
		return p;
	}

	private Path GetManhattanPath(Vector3 from, Vector3 dest) {
		Vector3 road = GetClosestRoadFrom(from, GetCoordinate().GetPos());
		Vector3 intersection = GetClosestIntersectionFromRoad(road, GetCoordinate().GetPos());

		Vector3 endRoad = GetClosestRoadFrom(dest, dest);
		Vector3 endIntersection = GetClosestIntersectionFromRoad(endRoad, dest);

		Vector3 delta = (endIntersection - intersection);
		Vector3 xResolve = intersection + Vector3.right * delta.x;

		// Debug.DrawLine(from, road, Color.cyan);
		// Debug.DrawLine(road, intersection, Color.red);
		// Debug.DrawLine(dest, endRoad, Color.cyan);
		// Debug.DrawLine(endIntersection, endRoad, Color.red);

		// Debug.DrawLine(intersection, xResolve, Color.white);
		// Debug.DrawLine(xResolve, xResolve + Vector3.up * delta.y, Color.white);

		return new Path(new List<PathSegment>() {
			new PathSegment(from, road, 0.5f),
			new PathSegment(road, intersection, 0.5f),
			new PathSegment(intersection, xResolve),
			new PathSegment(xResolve, endIntersection),
			new PathSegment(endIntersection, dest, 0.5f),
		});
	}

	private void WalkAlongPath(Coordinate targetCoord, Vector3 targetPos, System.Action reachedTarget, float moveSpeedModifier = 1.0f) {
		if (Util.manhattanDistance(GetCoordinate(), targetCoord) < 1) {
			walkingPath = Path.Empty();
			reachedTarget();
		}

		if (walkingPath.IsEmpty()) {
			// Debug.Log("No where to walk and didn't reach destination!");
		} else {
			// Walk towards next point in path
			transform.position = Vector3.MoveTowards(transform.position, walkingPath.NextDestination(), moveSpeed * moveSpeedModifier * Time.deltaTime);

			Debug.DrawLine(transform.position, walkingPath.NextDestination(), new Color(1, 0, 0, 0.2f));
			// for (int i = 0; i < walkingPath.Count() - 1; i++) Debug.DrawLine(walkingPath.DestinationAt(i), walkingPath.DestinationAt(i + 1), new Color(0, 0, 0, 0.2f));

			if (Vector3.Distance(transform.position, walkingPath.NextDestination()) < 0.01f) {
				// Remove point from walking path
				walkingPath.RemoveFirstSegment();
			}
		}
	}

	private void WalkStraightTo(Coordinate targetCoord, Vector3 targetPos, System.Func<Vector3, Vector3, bool> didReachTarget, System.Action reachedTarget, float moveSpeedModifier = 1.0f) {
		transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * moveSpeedModifier * Time.deltaTime);
		Debug.DrawLine(transform.position, targetPos, new Color(0, 1, 0, 0.6f));

		if (didReachTarget(transform.position, targetPos)) reachedTarget();
	}

	private void BeginIdlingIn(Coordinate coord) {
		idlePos = GetRandomPosInCoordinate(coord);
		idleTimer = 0;
	}

	private void IdleIn(Coordinate coord) {
		if (idleTimer > 2) {
			WalkStraightTo(coord, idlePos, (c1, c2) => Vector3.Distance(c1, c2) < 0.1f, () => {
				BeginIdlingIn(coord);
			}, 0.5f);
		}

		// for (int i = 0; i < 400; i++) {
		// 	Vector3 p = GetRandomPosInCoordinate(coord);
		// 	Debug.DrawLine(p + Vector3.up * 0.1f, p, Color.red);
		// }
	}

	private Vector3 GetClosestRoadFrom(Vector3 start, Vector3 end) {
		Vector3 vup = new Vector3(start.x, end.y, 0);
		Vector3 vdown = new Vector3(start.x, end.y - 4.0f, 0);
		Vector3 vleft = new Vector3(end.x, start.y, 0);
		Vector3 vright = new Vector3(end.x + 4.0f, start.y, 0);

		return Util.getClosestVector3(start, vup, vdown, vleft, vright);
	}

	private Vector3 GetClosestIntersectionFromRoad(Vector3 start, Vector3 end) {
		Vector3 iTopLeft = end;
		Vector3 iTopRight = end + Vector3.right * 4.0f;
		Vector3 iBottomLeft = end + Vector3.down * 4.0f;
		Vector3 iBottomRight = end + (Vector3.right + Vector3.down) * 4.0f;

		return Util.getClosestVector3(start, iTopLeft, iTopRight, iBottomLeft, iBottomRight);
	}

	private Coordinate GetCoordinate() {
		return GetCoordinate(transform.localPosition);
	}

	private Coordinate GetCoordinate(Vector3 pos) {
		return new Coordinate(Mathf.FloorToInt(pos.x / 4.0f), Mathf.FloorToInt(pos.y / -4.0f));
	}

	private Vector3 GetRandomPosInCoordinate(Coordinate c) {
		Vector3 topLeft = new Vector3(-23, 23, 0);

		float dx = Random.Range(0.5f, 3.5f);
		float dy = Random.Range(0.5f, 3.5f);

		return topLeft + new Vector3(c.x * 4.0f + dx, (c.y + 1) * -4.0f + dy, 0);
	}

	private string GenerateName() {
		string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string lowercase = "abcdefghijklmnopqrstuvwxyz";

		int firstNameLength = Random.Range(3, 7);
		int lastNameLength = Random.Range(3, 7);

		string generatedName = "";
		for (int i = 0; i < firstNameLength; i++) {
			if (i == 0) {
				generatedName += uppercase[Random.Range(0, uppercase.Length - 1)];
			} else {
				generatedName += lowercase[Random.Range(0, lowercase.Length - 1)];
			}
		}

		generatedName += " ";

		for (int i = 0; i < lastNameLength; i++) {
			if (i == 0) {
				generatedName += uppercase[Random.Range(0, uppercase.Length - 1)];
			} else {
				generatedName += lowercase[Random.Range(0, lowercase.Length - 1)];
			}
		}

		return generatedName;
	}
}
