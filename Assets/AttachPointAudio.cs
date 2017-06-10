using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPointAudio : MonoBehaviour
{
    public AudioClip[] Clips;
    private AudioSource _as;

    void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Random.Range(0.0f, 3.0f) > 2.5f && _as.isPlaying == false)
        {
            _as.clip = Clips[Random.Range(0, Clips.Length - 1)];
            _as.pitch = Random.Range(0.8f, 1.2f);
            _as.volume = Random.Range(0.5f, 1.0f);
            _as.Play();
        }
    }
}
