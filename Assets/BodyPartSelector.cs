using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSelector : MonoBehaviour
{
    private enum Status
    {
        Choosing,
        Dragging
    };

    private Status _status;
    private GameObject _target_part;
    private GameObject _dragged_object;

    void Start()
    {
        _status = Status.Choosing;
    }

    void Update()
    {
        switch(_status)
        {
            case Status.Choosing:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var bodyparts = GameObject.FindGameObjectsWithTag("Bodypart");
                        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        foreach (var bp in bodyparts)
                        {
                            if (bp == _target_part)
                                continue;

                            var ren = bp.GetComponent<SpriteRenderer>();
                            var mpb = new Vector3(mp.x, mp.y, ren.transform.position.z);
                            if (ren.bounds.Contains(mpb))
                            {
                                _status = Status.Dragging;
                                _dragged_object = bp;
                                break;
                            }
                        }
                    }
                }
                break;
            case Status.Dragging:
                {
                    Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _dragged_object.transform.position = mp;

                    if (Input.GetMouseButtonDown(0))
                    {
                        var doaps = new List<Transform>();

                        foreach (Transform c in _dragged_object.transform)
                        {
                            if (c.tag == "AttachPoint")
                                doaps.Add(c);
                        }

                        foreach (Transform tpap in _target_part.transform)
                        {
                            foreach (var doap in doaps)
                            {
                                if ((tpap.position - doap.position).magnitude < 1)
                                {
                                    doap.parent = tpap.parent;
                                    Done();
                                    return;
                                }
                            }
                        }
                    }
                }
                break;
        }
        
    }

    void Done()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void InititateSelection(GameObject bodypart)
    {
        _target_part = bodypart;
        Time.timeScale = 0;
        Cursor.visible = true;
    }
}
