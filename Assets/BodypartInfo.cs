using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartInfo : MonoBehaviour
{
    public bool IsLegArm = false;
    public List<GameObject> MetaBody = null;

    void Start()
    {
        MetaBody = null;
    }
}
