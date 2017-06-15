using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudNice : MonoBehaviour
{
    private Vector2 _org_pos;

    void Start()
    {
        _org_pos = transform.position;
    }

    void Update()
    {
        transform.position = _org_pos + new Vector2(Mathf.Sin(Time.time * 0.1f), Mathf.Sin(Time.time) * Mathf.Cos(Time.time * 0.05f) * 0.05f);
    }
}
