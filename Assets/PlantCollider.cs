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
    }

    private bool CanConnectAttachpoints(GameObject obj)
    {
        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        if (attachpoints.Length < 2)
            return false;

        int counter = attachpoints.Length;
        var points_in_part = BodyPartSelector.FindAllAttachPoints(obj);
        counter -= points_in_part.Count;
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

        var halo = transform.Find("Halo").GetComponent<HaloAnimator>();
        halo.EnterGoingToBody(other.gameObject);

        var bps = Resources.Load("BodyPartSelector") as GameObject;
        var go = Instantiate(bps);
        go.GetComponent<BodyPartSelector>().InititateSelection(other.gameObject, gameObject);
    }
}
