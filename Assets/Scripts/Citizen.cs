using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour {
	private enum State {
		MOVING_IN,
		AT_HOME,
		GOING_TO_WORK,
		AT_WORK,
		GOING_HOME,
	}

	private State state;

	private SpriteRenderer spriteR;
	private string citizenName;

	private ResidentialCityBlock home;
	private Vector3 idlePos;

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

		Coordinate homeCoordinate = home.GetComponent<Block>().pos;
		Coordinate workCoordinate = work.GetComponent<Block>().pos;
		switch (state) {
			case State.MOVING_IN:
				transform.position = Vector3.MoveTowards(transform.position, home.transform.position, moveSpeed * 5 * Time.deltaTime);
				Debug.DrawLine(transform.position, home.transform.position, Color.red);

				if (Util.manhattanDistance(GetCoordinate(), homeCoordinate) < 1) {
					state = State.AT_HOME;

					stateTimer = 0;
					idleTimer = 2;
					idlePos = GetRandomPosInCoordinate(homeCoordinate);
				}

				break;
			case State.AT_HOME:
				// for (int i = 0; i < 400; i++) {
				// 	// home.GetComponent<Block>().pos
				// 	Vector3 p = GetRandomPosInCoordinate(new Coordinate(4, 0));
				// 	Debug.DrawLine(p + Vector3.up * 0.1f, p, Color.red);
				// }

				if (idleTimer > 2) {
					transform.position = Vector3.MoveTowards(transform.position, idlePos, moveSpeed * 0.5f * Time.deltaTime);
					Debug.DrawLine(transform.position, idlePos, Color.red);
				}

				if (Vector3.Distance(transform.position, idlePos) < 0.1f) {
					idlePos = GetRandomPosInCoordinate(homeCoordinate);
					idleTimer = 0;
				}

				if (stateTimer > 10.0f) {
					state = State.GOING_TO_WORK;
					stateTimer = 0;
					idleTimer = 2;
				}

				break;
			case State.GOING_TO_WORK:
				transform.position = Vector3.MoveTowards(transform.position, work.transform.position, moveSpeed * Time.deltaTime);
				Debug.DrawLine(transform.position, work.transform.position, Color.red);

				if (Util.manhattanDistance(GetCoordinate(), workCoordinate) < 1) {
					state = State.AT_WORK;
					stateTimer = 0;
					idleTimer = 2;
					idlePos = GetRandomPosInCoordinate(workCoordinate);
				}

				break;
			case State.AT_WORK:
				if (idleTimer > 2) {
					transform.position = Vector3.MoveTowards(transform.position, idlePos, moveSpeed * 0.5f * Time.deltaTime);
					Debug.DrawLine(transform.position, idlePos, Color.red);
				}

				if (Vector3.Distance(transform.position, idlePos) < 0.1f) {
					idlePos = GetRandomPosInCoordinate(workCoordinate);
					idleTimer = 0;
				}

				if (stateTimer > 10.0f) {
					state = State.GOING_HOME;
					stateTimer = 0;
					idleTimer = 2;
				}

				break;
			case State.GOING_HOME:
				transform.position = Vector3.MoveTowards(transform.position, home.transform.position, moveSpeed * Time.deltaTime);
				Debug.DrawLine(transform.position, home.transform.position, Color.red);

				if (Util.manhattanDistance(GetCoordinate(), homeCoordinate) < 1) {
					state = State.AT_HOME;

					stateTimer = 0;
					idleTimer = 2;
					idlePos = GetRandomPosInCoordinate(homeCoordinate);
				}

				break;
		}
		// Debug.DrawLine(transform.position, home.transform.position, Color.blue);
		// Debug.DrawLine(transform.position, work.transform.position, Color.red);
	}

	public void SetHome(ResidentialCityBlock home) {
		this.home = home;

		// float dx = Random.Range(-0.5f, 0.5f);
		// float dy = Random.Range(-0.5f, 0.5f);
		// targetSpot = home.transform.position + Vector3.right * dx + Vector3.up * dy;
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

	private Coordinate GetCoordinate() {
		return new Coordinate(Mathf.FloorToInt(transform.localPosition.x / 4.0f), Mathf.FloorToInt(transform.localPosition.y / -4.0f));
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
