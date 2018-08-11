using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayBlock : MonoBehaviour {
	[SerializeField] private Sprite emptySprite;
	[SerializeField] private Sprite horizontalSprite;
	[SerializeField] private Sprite nodeDownSprite;
	[SerializeField] private Sprite nodeLeftSprite;
	[SerializeField] private Sprite nodeRightSprite;
	[SerializeField] private Sprite nodeUpSprite;
	[SerializeField] private Sprite verticalSprite;

	public enum State {
		EMPTY,
		HORIZONTAL,
		NODE_DOWN,
		NODE_LEFT,
		NODE_RIGHT,
		NODE_UP,
		VERTICAL,
	}

	private SpriteRenderer spriteR;
	private State state;

	public void SetState(State newState) {
		state = newState;
	}

	void Start () {
		spriteR = GetComponent<SpriteRenderer>();
		state = State.EMPTY;
	}

	void Update () {
		switch (state) {
			case State.EMPTY:
				spriteR.sprite = emptySprite;
				break;
			case State.HORIZONTAL:
				spriteR.sprite = horizontalSprite;
				break;
			case State.NODE_DOWN:
				spriteR.sprite = nodeDownSprite;
				break;
			case State.NODE_LEFT:
				spriteR.sprite = nodeLeftSprite;
				break;
			case State.NODE_RIGHT:
				spriteR.sprite = nodeRightSprite;
				break;
			case State.NODE_UP:
				spriteR.sprite = nodeUpSprite;
				break;
			case State.VERTICAL:
				spriteR.sprite = verticalSprite;
				break;
		}
	}
}
