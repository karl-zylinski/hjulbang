using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float ParallaxAmount = 0.0f;
    void Start()
    {
        
    }

    void Update()
    {
        Vector2 pos = transform.position;
        pos.x = Camera.main.transform.position.x * ParallaxAmount;
        transform.position = pos;
    }
}
