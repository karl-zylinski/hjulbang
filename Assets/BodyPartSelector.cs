using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSelector : MonoBehaviour
{
    private enum Status
    {
        Choosing,
        Dragging,
        Done
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

                                    if (attach_to_info.IsLegArm && !attacher_info.IsLegArm)
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

                                    if (!attach_to_info.IsLegArm)
                                        Destroy(attach_to_bpc);

                                    if (!attacher_info.IsLegArm)
                                        Destroy(attacher_bpc);

                                    if (attacher_bpc)
                                        attacher_bpc.ForceMultiplier = 2.9f;

                                    CreateOrMergeMetabodies(attacher_info, attach_to_info);
                                    Done();
                                    attacher_ap.gameObject.tag = "Untagged";
                                    attach_to_ap.gameObject.tag = "Untagged";
                                    return;
                                }
                            }
                        }
                    }
                }
                break;
            case Status.Done:
                break;
        }
    }

    private void CreateOrMergeMetabodies(BodypartInfo part1, BodypartInfo part2)
    {
        var metabody = part1.MetaBody;
        var old_metabody = part2.MetaBody;

        foreach (var part in old_metabody)
        {
            var bpi = part.GetComponent<BodypartInfo>();
            bpi.MetaBody = metabody;
            metabody.Add(part);
        }

        part2.MetaBody = metabody;
        old_metabody.Clear();
    }

    void Done()
    {
        _status = Status.Done;
        SetAttachpointVisible(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        _plant.GetComponent<PlantCollider>().SetUsed();

        Destroy(gameObject);
    }

    private DraggedObject CreateDraggedObject(GameObject attachpoint)
    {
        var part = attachpoint.transform.parent.gameObject;
        var bpi = part.GetComponent<BodypartInfo>();
        List<DraggedSubObject> dsos = new List<DraggedSubObject>();
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (var metapart in bpi.MetaBody)
        {
            var mbpi = metapart.GetComponent<BodypartInfo>();
            Vector2 pp = metapart.transform.position;
            DraggedSubObject dso = new DraggedSubObject()
            {
                Obj = metapart,
                PosDiff = pp - mp
            };
            dsos.Add(dso);
        }

        return new DraggedObject()
        {
            Items = dsos,
            AttachPoint = attachpoint
        };
    }

    private void AddAllDraggedParts(List<DraggedSubObject> dsos, GameObject obj)
    {
        var bpi = obj.GetComponent<BodypartInfo>();
        Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (var part in bpi.MetaBody)
        {
            Vector2 pp = part.transform.position;

            DraggedSubObject dso = new DraggedSubObject()
            {
                Obj = part,
                PosDiff = pp - mp
            };

            dsos.Add(dso);
        }
    }

    public static void AddAllAttachPoints(List<GameObject> attachpoints, GameObject obj)
    {
        foreach (Transform c in obj.transform)
        {
            if (c.tag == "AttachPoint")
                attachpoints.Add(c.gameObject);
        }
    }

    public static List<GameObject> FindAllAttachPoints(GameObject part)
    {
        var bpi = part.GetComponent<BodypartInfo>();
        var attachpoints = new List<GameObject>();
        
        foreach(var obj in bpi.MetaBody)
        {
            AddAllAttachPoints(attachpoints, obj);
        }

        return attachpoints;
    }
    
    private TargetObject CreateTargetObject(GameObject part)
    {
        return new TargetObject()
        {
            AttachPoints = FindAllAttachPoints(part)
        };
    }

    private void SetAttachpointVisible(bool visible)
    {
        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        foreach (var a in attachpoints)
        {
            var sr = a.GetComponent<SpriteRenderer>();
            sr.enabled = visible;
            sr.color = new Color(1, 0, 0, 1);
        }
    }

    public void InititateSelection(GameObject bodypart, GameObject plant)
    {
        SetAttachpointVisible(true);
        Camera.main.GetComponent<CameraControl>().Focus(bodypart);
        _target_object = CreateTargetObject(bodypart);
        foreach (var a in _target_object.AttachPoints)
        {
            var sr = a.GetComponent<SpriteRenderer>();
            sr.color = new Color(0, 1, 0, 1);
        }
        _plant = plant;
        Time.timeScale = 0;
        Cursor.visible = true;
    }
}
