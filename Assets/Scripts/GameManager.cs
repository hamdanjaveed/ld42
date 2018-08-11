using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Some sort of currency
// Some penalty where editing a previous path ("construction"), maybe public opinion goes down
// Adding stations
// Think more about 1 path serving more than 2 buildings
// Maybe add zones and different types of buildings with different demand
// Upgrading paths for more throughput
// Some prevention of cheating the system by just building 1 big subway (maybe the faster the subway the better public opinion (higher score??))

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
		SwitchToCity();
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
				break;
			case GameView.SUBWAY:
				SwitchToCity();
				break;
		}
	}

	void SwitchToSubway() {
		currentGameView = GameView.SUBWAY;
		cityManager.SetActive(false);
		subwayManager.SetActive(true);
	}

	void SwitchToCity() {
		currentGameView = GameView.CITY;
		cityManager.SetActive(true);
		subwayManager.SetActive(false);
	}
}
