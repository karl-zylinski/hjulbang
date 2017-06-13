using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
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
    }
}
