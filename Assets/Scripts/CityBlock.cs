using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock : Block {
    public bool occupied;

    private SpriteRenderer spriteR;

    public void Choose() {
        spriteR.color = new Color(0.5f, 0.2f, 0.7f);
    }

    void Start() {
        occupied = false;
        spriteR = gameObject.GetComponent<SpriteRenderer>();
    }

}
