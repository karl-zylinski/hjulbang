using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsMusicTrigger : MonoBehaviour
{
    private float _fade;
    private bool _started;
    private bool _done;

    void Start()
    {
        _fade = 0;
        _started = false;
        _done = false;
    }

    void Update()
    {
        if (_done)
            return;

        if (!_started)
            return;

        _fade += Time.unscaledDeltaTime / 4.0f;

        if (_fade > 1)
        {
            _fade = 1;
            _done = true;
        }

        GetComponent<AudioSource>().volume = _fade;
        GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().volume = 1 - _fade;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart" || _started)
            return;

        _started = true;
        GetComponent<AudioSource>().Play();
        Camera.main.GetComponent<CameraControl>().Focus(other.gameObject);
    }
}
