using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[SerializeField] private GameObject cityManager;
	[SerializeField] private GameObject subwayManager;

	enum GameView {
		CITY,
		SUBWAY,
	}

	private GameView currentGameView;

	// Use this for initialization
	void Start () {
		currentGameView = GameView.SUBWAY;
		SwitchToSubway();
	}

	void Update() {
		if (Input.GetKeyDown("q")) {
			ToggleGameView();
		}
	}

	void ToggleGameView() {
		switch (currentGameView) {
			case GameView.CITY:
				SwitchToSubway();
				currentGameView = GameView.SUBWAY;
				break;
			case GameView.SUBWAY:
				SwitchToCity();
				currentGameView = GameView.CITY;
				break;
		}
	}

	void SwitchToSubway() {
		cityManager.SetActive(false);
		subwayManager.SetActive(true);
	}

	void SwitchToCity() {
		cityManager.SetActive(true);
		subwayManager.SetActive(false);
	}
}
