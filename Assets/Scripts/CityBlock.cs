using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : Block {
	private SpriteRenderer spriteR;

	public void Choose() {
		spriteR.color = new Color(0.5f, 0.2f, 0.7f);
	}

	void Start() {
		spriteR = gameObject.GetComponent<SpriteRenderer>();
	}

}
