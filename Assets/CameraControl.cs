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

        if (Input.GetKey(KeyCode.RightShift))
            x = x * 3;

        var new_pos = transform.position + new Vector3(x, 0, 0) * Time.unscaledDeltaTime * 10;

        var parts = GameObject.FindGameObjectsWithTag("Bodypart");
        var min_x = parts[0].transform.position.x;
        var max_x = parts[0].transform.position.x;

        foreach (var p in parts)
        {
            if (p.transform.position.x < min_x)
                min_x = p.transform.position.x;
            else if (p.transform.position.x > max_x)
                max_x = p.transform.position.x;
        }

        new_pos.x = Mathf.Clamp(new_pos.x, min_x - 3, max_x + 3);
        Camera.main.transform.position = new_pos;
    }
}
