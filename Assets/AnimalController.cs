using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public GameObject[] Legs;
    private float _speed;

    void Start()
    {
        _speed = 0;
    }

    void Update()
    {
        var up_held = Input.GetKey(KeyCode.UpArrow);
        var down_held = Input.GetKey(KeyCode.DownArrow);

        if (up_held)
            _speed = _speed + 1;

        if (down_held)
            _speed = _speed - 1;

        foreach (var leg in Legs)
        {
            leg.transform.Rotate(0, 0, _speed * Time.deltaTime);
        }
    }
}
