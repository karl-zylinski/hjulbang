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

    private struct DraggedSubObject
    {
        public Vector2 PosDiff;
        public GameObject Obj;
    };

    private struct DraggedObject
    {
        public GameObject AttachPoint;
        public List<DraggedSubObject> Items;
    };

    private struct TargetSubObject
    {
        public GameObject Obj;
        public List<GameObject> AttachPoint;
    };

    private struct TargetObject
    {
        public List<GameObject> AttachPoints;
    };

    private Status _status;
    private TargetObject _target_object;
    private DraggedObject _dragged_object;
    private GameObject _plant;

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
                        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");
                        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        foreach (var a in attachpoints)
                        {
                            if (_target_object.AttachPoints.FindIndex((x) => x == a) != -1)
                                continue;

                            var ren = a.GetComponent<SpriteRenderer>();
                            var mpb = new Vector3(mp.x, mp.y, ren.transform.position.z);
                            if (ren.bounds.Contains(mpb))
                            {
                                var d = CreateDraggedObject(a);
                                _status = Status.Dragging;
                                _dragged_object = d;
                                break;
                            }
                        }
                    }
                }
                break;
            case Status.Dragging:
                {
                    Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    foreach (var i in _dragged_object.Items)
                    {
                        i.Obj.transform.position = mp + i.PosDiff;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        foreach (GameObject toap in _target_object.AttachPoints)
                        {
                            var tpap = _dragged_object.AttachPoint;

                            foreach (var di in _dragged_object.Items)
                            {
                                if ((tpap.transform.position - toap.transform.position).magnitude < 0.5f)
                                {
                                    var obj1 = tpap.transform.parent;
                                    var obj2 = toap.transform.parent;

                                    var attach_to = obj1;
                                    var attach_to_ap = tpap;
                                    var attacher = obj2;
                                    var attacher_ap = toap;

                                    var attach_to_info = attach_to.GetComponent<BodypartInfo>();
                                    var attacher_info = attacher.GetComponent<BodypartInfo>();

                                    if ((attach_to_info.IsLegArm && !attacher_info.IsLegArm))
                                    {
                                        attach_to = obj2;
                                        attacher = obj1;
                                        attach_to_ap = toap;
                                        attacher_ap = tpap;
                                    }

                                    var attach_to_bpc = attach_to.GetComponent<BodypartController>();
                                    var attacher_bpc = attacher.GetComponent<BodypartController>();
                                    attach_to_info = attach_to.GetComponent<BodypartInfo>();
                                    attacher_info = attacher.GetComponent<BodypartInfo>();
                                    var none_is_leg = !attacher_info.IsLegArm && !attach_to_info.IsLegArm;

                                    if (none_is_leg)
                                    {
                                        var fj = attach_to.gameObject.AddComponent<FixedJoint2D>();
                                        fj.connectedBody = attacher.gameObject.GetComponent<Rigidbody2D>();
                                        fj.anchor = attach_to_ap.transform.localPosition;
                                        fj.autoConfigureConnectedAnchor = false;
                                        fj.connectedAnchor = attacher_ap.transform.localPosition;
                                    }
                                    else
                                    {
                                        var hj = attach_to.gameObject.AddComponent<HingeJoint2D>();
                                        hj.connectedBody = attacher.gameObject.GetComponent<Rigidbody2D>();
                                        hj.anchor = attach_to_ap.transform.localPosition;
                                        hj.autoConfigureConnectedAnchor = false;
                                        hj.connectedAnchor = attacher_ap.transform.localPosition;
                                    }

                                    attacher.position = attach_to_ap.transform.position + (attacher_ap.transform.localPosition);

                                    if (attach_to_bpc)
                                        Destroy(attach_to_bpc);

                                    if (attacher_bpc)
                                        attacher_bpc.ForceMultiplier = 2.9f;

                                    attacher_info.Parent = attach_to.gameObject;
                                    attach_to_info.Children.Add(attacher.gameObject);
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
        SetAttachpointVisible(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        Destroy(_plant);
        Destroy(gameObject);
    }

    private DraggedObject CreateDraggedObject(GameObject attachpoint)
    {
        var part = attachpoint.transform.parent.gameObject;
        var root = FindRootPart(part);
        var bpi = root.GetComponent<BodypartInfo>();
        
        List<DraggedSubObject> dsos = new List<DraggedSubObject>();
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pp = root.transform.position;

        DraggedSubObject root_dso = new DraggedSubObject()
        {
            Obj = root,
            PosDiff = pp - mp
        };

        dsos.Add(root_dso);
        AddAllDraggedChildren(dsos, root);
        return new DraggedObject()
        {
            Items = dsos,
            AttachPoint = attachpoint
        };
    }

    private void AddAllDraggedChildren(List<DraggedSubObject> dsos, GameObject obj)
    {
        var parent_info = obj.GetComponent<BodypartInfo>();
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (var child in parent_info.Children)
        {
            Vector2 pp = child.transform.position;

            DraggedSubObject dso = new DraggedSubObject()
            {
                Obj = child,
                PosDiff = pp - mp
            };
            
            dsos.Add(dso);
            AddAllDraggedChildren(dsos, child);
        }
    }

    private void AddAllTargetAttachpointChildren(List<GameObject> attachpoints, GameObject obj)
    {
        var parent_info = obj.GetComponent<BodypartInfo>();
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (var physics_child in parent_info.Children)
        {
            foreach (Transform trans_child in physics_child.transform)
            {
                if (trans_child.gameObject.tag == "AttachPoint")
                    attachpoints.Add(trans_child.gameObject);
            }

            AddAllTargetAttachpointChildren(attachpoints, physics_child);
        }
    }

    private List<GameObject> FindAllAttachPoints(GameObject obj)
    {
        var attachpoints = new List<GameObject>();

        foreach (Transform c in obj.transform)
        {
            if (c.tag == "AttachPoint")
                attachpoints.Add(c.gameObject);
        }

        return attachpoints;
    }

    public static void AddAllAttachPointsInTree(GameObject obj, List<GameObject> attachpoints)
    {
        foreach (Transform c in obj.transform)
        {
            if (c.tag == "AttachPoint")
                attachpoints.Add(c.gameObject);

            AddAllAttachPointsInTree(c.gameObject, attachpoints);
        }
    }

    public static List<GameObject> FindAllAttachPointsInTree(GameObject obj)
    {
        var attachpoints = new List<GameObject>();
        AddAllAttachPointsInTree(obj, attachpoints);
        return attachpoints;
    }

    public static GameObject FindRootPart(GameObject part)
    {
        var bpi = part.GetComponent<BodypartInfo>();

        var p = bpi.Parent;
        if (p == null)
        {
            p = part;
        }
        else
        {
            while (true)
            {
                var parents_parent = p.GetComponent<BodypartInfo>().Parent;

                if (parents_parent == null)
                    break;

                p = parents_parent;
            }
        }

        return p;
    }

    private TargetObject CreateTargetObject(GameObject part)
    {
        var root = FindRootPart(part);
        List<GameObject> attachpoints = new List<GameObject>();
        attachpoints.AddRange(FindAllAttachPoints(root));
        AddAllTargetAttachpointChildren(attachpoints, root);

        return new TargetObject()
        {
            AttachPoints = attachpoints
        };
    }

    private void SetAttachpointVisible(bool visible)
    {
        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        foreach (var a in attachpoints)
        {
            a.GetComponent<SpriteRenderer>().enabled = visible;
        }
    }

    public void InititateSelection(GameObject bodypart, GameObject plant)
    {
        SetAttachpointVisible(true);
        _target_object = CreateTargetObject(bodypart);
        _plant = plant;
        Time.timeScale = 0;
        Cursor.visible = true;
    }
}
