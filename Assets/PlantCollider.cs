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

    public void SetUsed()
    {
        Destroy(GetComponent<Collider2D>());
        Destroy(transform.GetChild(0).gameObject);
    }

    private bool CanConnectAttachpoints(GameObject obj)
    {
        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        if (attachpoints.Length < 2)
            return false;

        int counter = attachpoints.Length;
        var bpi = obj.GetComponent<BodypartInfo>();

        if (bpi.MetaBody == null)
            return true;
        else
        {
            foreach (var part in bpi.MetaBody)
            {
                var points_in_part = BodyPartSelector.FindAllAttachPoints(part);
                counter -= points_in_part.Count;
            }
        }

        return counter > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart")
            return;

        if (!CanConnectAttachpoints(other.gameObject))
            return;

        var attach_points_for_obj = BodyPartSelector.FindAllAttachPoints(other.gameObject);

        if (attach_points_for_obj.Count == 0)
            return;

        var bps = Resources.Load("BodyPartSelector") as GameObject;
        var go = Instantiate(bps);
        go.GetComponent<BodyPartSelector>().InititateSelection(other.gameObject, gameObject);
    }
}
