﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public Sprite[] Backgrounds;
    public int Order = 0;
    private GameObject _current_front;
    public float YOffset = 0;
    public float XOverlap = 0;

    private Sprite GetNewBackground()
    {
        return Backgrounds[Random.Range(0, Backgrounds.Length - 1)];
    }

    private Vector2 GetNewFrontPos(Sprite new_sprite)
    {
        if (_current_front == null)
        {
            return new Vector2(20.0f, YOffset);
        }

        var x = XOverlap + _current_front.transform.position.x - _current_front.GetComponent<SpriteRenderer>().bounds.size.x / 2 - new_sprite.bounds.size.x / 2;
        return new Vector2(x, YOffset);
    }

    private void CreateNewFront()
    {
        var sprite = GetNewBackground();
        var new_pos = GetNewFrontPos(sprite);
        var bg = new GameObject("bg");
        var sr = bg.AddComponent<SpriteRenderer>();
        bg.transform.parent = transform;
        sr.sprite = sprite;
        sr.sortingOrder = Order;
        bg.transform.position = new_pos;
        _current_front = bg;
    }

    void Start()
    {
        CreateNewFront();
        CreateNewFront();
    }

    void Update()
    {
        Vector2 cp = Camera.main.transform.position;
        var cfb = _current_front.GetComponent<SpriteRenderer>().bounds;
        var dist = cfb.min.x - (cp.x - Camera.main.orthographicSize * Camera.main.aspect);
        if (Mathf.Abs(dist) < 1)
        {
            CreateNewFront();
        }
    }
}
