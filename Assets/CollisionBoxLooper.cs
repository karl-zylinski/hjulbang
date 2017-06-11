using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBoxLooper : MonoBehaviour
{
    public float YPos = 0;
    public float Height = 2;
    private GameObject _current_front;
    public PhysicsMaterial2D GroundMaterial;

    void Start()
    {
        CreateNewFront();
        CreateNewFront();
    }

    private Vector2 GetNewFrontPos(BoxCollider2D new_collider)
    {
        if (_current_front == null)
        {
            return new Vector2(20.0f, YPos);
        }

        var x = _current_front.transform.position.x - _current_front.GetComponent<BoxCollider2D>().bounds.size.x / 2 - new_collider.bounds.size.x / 2;
        return new Vector2(x, YPos);
    }

    private void CreateNewFront()
    {
        var go = new GameObject("ground collider");
        go.tag = "Ground";
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(24, 3);
        bc.sharedMaterial = GroundMaterial;
        go.transform.parent = transform;
        go.transform.position = GetNewFrontPos(bc);
        _current_front = go;
    }

    void Update()
    {
        Vector2 cp = Camera.main.transform.position;
        var front_metabody = Camera.main.GetComponent<CameraControl>().MetabodiesByXCoord[0];

        if (front_metabody.Count == 0)
            return;

        var front_body = front_metabody[0];

        while (true)
        {
            var cfb = _current_front.GetComponent<BoxCollider2D>().bounds;
            var dist = cfb.min.x - (front_body.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect);
            if (Mathf.Abs(dist) < 1)
            {
                CreateNewFront();
            }
            else
                break;
        }
    }
}
