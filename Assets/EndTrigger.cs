using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private bool _used;

    void Start()
    {
        _used = false;
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart" || _used)
            return;

        var part = other.gameObject;
        var metabody = part.GetComponent<BodypartInfo>().MetaBody;
        Destroy(Camera.main.GetComponent<CameraControl>());
        var end_screen = GameObject.Find("EndScreen");
        var end_screen_start = end_screen.transform.Find("start");
        var origin = metabody[0].transform.position;
        
        foreach (var p in metabody)
        {
            p.transform.position = end_screen_start.transform.position + (p.transform.position - origin);
        }

        var all_plants = GameObject.FindGameObjectsWithTag("Plant");

        foreach (var p in all_plants)
        {
            Destroy(p);
        }

        Camera.main.transform.position = new Vector3(end_screen.transform.position.x, end_screen.transform.position.y, -10);
        _used = true;
    }
}
