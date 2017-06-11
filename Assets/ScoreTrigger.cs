using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D other)
    {
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

        var tm = gameObject.GetComponentInChildren<TextMesh>();
        tm.text = "Darwin thinks you're worth " + metabody.Count.ToString() + " points";
    }
}
