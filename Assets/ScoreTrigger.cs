using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScoreTrigger : MonoBehaviour
{
    private bool _started;
    private float _started_at;

    void Start()
    {
        _started = false;
    }

    void Update()
    {
        if (_started && Time.time > _started_at + 2.0f && (Input.GetKeyDown(KeyCode.Return)))
        {
            SceneManager.LoadScene("Start");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_started)
            return;

        if (other.gameObject.tag != "Bodypart")
            return;

        var part = other.gameObject;
        var metabody = part.GetComponent<BodypartInfo>().MetaBody;

        foreach (var p in metabody)
        {
            var bpc = p.GetComponent<BodypartController>();

            if (bpc != null)
                Destroy(bpc);
        }

        GetComponent<AudioSource>().Play();
        var tm = gameObject.GetComponentInChildren<TextMesh>();
        tm.text = "Darwin thinks you're worth " + metabody.Count.ToString() + " points";
        _started = true;
        _started_at = Time.time;
    }
}
