using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        var x = 0;
        var y = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
            x -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            x += 1;
        if (Input.GetKey(KeyCode.UpArrow))
            y += 1;
        if (Input.GetKey(KeyCode.DownArrow))
            y -= 1;
        transform.position = transform.position + new Vector3(x, y, 0) * Time.unscaledDeltaTime * 10;
    }
}
