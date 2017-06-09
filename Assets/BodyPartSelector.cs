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

                            var bpi = bp.GetComponent<BodypartInfo>();
                            var ren = bp.GetComponent<SpriteRenderer>();
                            var mpb = new Vector3(mp.x, mp.y, ren.transform.position.z);
                            if (ren.bounds.Contains(mpb))
                            {
                                var cbp = bp;
                                _status = Status.Dragging;
                                var parent = bpi.Parent;
                                if (parent != null)
                                {
                                    while (true)
                                    {
                                        var parents_parent = parent.GetComponent<BodypartInfo>().Parent;

                                        if (parents_parent == null)
                                            break;

                                        parent = parents_parent;
                                        cbp = parent;
                                    }
                                }
                                _dragged_object = cbp;
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
                            if (tpap.tag != "AttachPoint")
                                continue;

                            foreach (var doap in doaps)
                            {
                                if ((tpap.position - doap.position).magnitude < 0.5f)
                                {
                                    var obj1 = tpap.parent;
                                    var obj2 = doap.parent;

                                    var attach_to = obj1;
                                    var attach_to_ap = tpap;
                                    var attacher = obj2;
                                    var attacher_ap = doap;

                                    var attach_to_info = attach_to.GetComponent<BodypartInfo>();
                                    var attacher_info = attacher.GetComponent<BodypartInfo>();

                                    if (attach_to_info.IsLegArm && !attacher_info.IsLegArm)
                                    {
                                        attach_to = obj2;
                                        attacher = obj1;
                                        attach_to_ap = doap;
                                        attacher_ap = tpap;
                                    }

                                    var attach_to_bpc = attach_to.GetComponent<BodypartController>();
                                    var attacher_bpc = attacher.GetComponent<BodypartController>();
                                    attach_to_info = attach_to.GetComponent<BodypartInfo>();
                                    attacher_info = attacher.GetComponent<BodypartInfo>();

                                    var hj = attach_to.gameObject.AddComponent<HingeJoint2D>();
                                    hj.connectedBody = attacher.gameObject.GetComponent<Rigidbody2D>();
                                    hj.anchor = attach_to_ap.localPosition;
                                    attacher.position = attach_to_ap.position + (attacher_ap.localPosition);

                                    if (attach_to_bpc)
                                        Destroy(attach_to_bpc);

                                    if (attacher_bpc)
                                        attacher_bpc.ForceMultiplier = 2.0f;

                                    attacher_info.Parent = attach_to.gameObject;
                                    Destroy(attacher_ap.gameObject);
                                    Destroy(attach_to_ap.gameObject);
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
