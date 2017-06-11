using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarwinBob : MonoBehaviour
{
    private Vector3 _start_pos;

    void Start()
    {
        _start_pos = transform.position;
    }

    void Update()
    {
        transform.position = _start_pos + new Vector3(0, Mathf.Sin(Time.unscaledTime) * 0.3f, 0);
    }
}
