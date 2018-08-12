using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityClock : MonoBehaviour {
	private const float oneCityHourInSeconds = 3;

	[SerializeField] GameEvent goToWork;
	[SerializeField] GameEvent goHome;

	[SerializeField] Text timeLabel;

	float timer;
	byte hour;

	void Start() {
		timer = 0;
		hour = 0;

		UpdateTimeLabel();
	}

	void Update () {
		timer += Time.deltaTime;
		if (timer >= oneCityHourInSeconds) {
			timer = 0;

			// Increment hour
			hour++;
			if (hour > 23) hour = 0;

			if (hour == 8) {			// 8 am
				goToWork.Signal();
			} else if (hour == 17) {	// 5 pm
				goHome.Signal();
			}

			UpdateTimeLabel();
		}
	}

	public override string ToString() {
		string desc = (hour % 12 == 0 ? 12 : hour % 12).ToString();
		if (hour < 12) {
			return desc + " AM";
		} else {
			return desc + " PM";
		}
	}

	private void UpdateTimeLabel() {
		timeLabel.text = ToString();
	}
}
