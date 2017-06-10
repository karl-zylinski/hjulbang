using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCollider : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private bool CanConnectAttachpoints()
    {
        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        if (attachpoints.Length < 2)
            return false;

        var known_roots = new List<GameObject>();

        foreach(var a in attachpoints)
        {
            var root = BodyPartSelector.FindRootPart(a.transform.parent.gameObject);
            if (!known_roots.Contains(root))
                known_roots.Add(root);
        }

        return known_roots.Count >= 2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart")
            return;

        if (!CanConnectAttachpoints())
            return;

        var bps = Resources.Load("BodyPartSelector") as GameObject;
        var go = Instantiate(bps);
        go.GetComponent<BodyPartSelector>().InititateSelection(other.gameObject, gameObject);
    }
}
