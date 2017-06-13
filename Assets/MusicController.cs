using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private bool _started;

    void Start()
    {
        _started = false;
    }

    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart" || _started)
            return;

        GetComponent<AudioSource>().Play();
        _started = true;
    }
}
