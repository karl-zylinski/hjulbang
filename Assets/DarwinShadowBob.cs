using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarwinShadowBob : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.localScale = new Vector3(1, 1, 1) * (Mathf.Sin(-Time.unscaledTime) + 4.0f)/5.0f;
    }
}
