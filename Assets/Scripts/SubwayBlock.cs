using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayBlock : Block {
    public enum State {
        CORNER_DOWN_LEFT,
        CORNER_LEFT_UP,
        CORNER_RIGHT_DOWN,
        CORNER_UP_RIGHT,
        EMPTY,
        HORIZONTAL,
        NODE,
        NODE_DOWN,
        NODE_LEFT,
        NODE_RIGHT,
        NODE_UP,
        VERTICAL,
    }

    public State state;
    public bool confirmed;

    [SerializeField] private Sprite cornerDownLeftSprite;
    [SerializeField] private Sprite cornerLeftUpSprite;
    [SerializeField] private Sprite cornerRightDownSprite;
    [SerializeField] private Sprite cornerUpRightSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite horizontalSprite;
    [SerializeField] private Sprite nodeSprite;
    [SerializeField] private Sprite nodeDownSprite;
    [SerializeField] private Sprite nodeLeftSprite;
    [SerializeField] private Sprite nodeRightSprite;
    [SerializeField] private Sprite nodeUpSprite;
    [SerializeField] private Sprite verticalSprite;

    private SpriteRenderer spriteR;

    void Start () {
        spriteR = GetComponent<SpriteRenderer>();
        state = State.EMPTY;
        confirmed = false;
    }

    void Update () {
        switch (state) {
            case State.CORNER_DOWN_LEFT:
                spriteR.sprite = cornerDownLeftSprite;
                break;
            case State.CORNER_LEFT_UP:
                spriteR.sprite = cornerLeftUpSprite;
                break;
            case State.CORNER_RIGHT_DOWN:
                spriteR.sprite = cornerRightDownSprite;
                break;
            case State.CORNER_UP_RIGHT:
                spriteR.sprite = cornerUpRightSprite;
                break;
            case State.EMPTY:
                spriteR.sprite = emptySprite;
                break;
            case State.HORIZONTAL:
                spriteR.sprite = horizontalSprite;
                break;
            case State.NODE:
                spriteR.sprite = nodeSprite;
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

        if (state != State.EMPTY && !confirmed) {
            spriteR.color = new Color(0.8f, 0.95f, 0.8f, 0.9f);
        } else {
            spriteR.color = Color.white;
        }
    }
}
