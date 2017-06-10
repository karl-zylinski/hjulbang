using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartInfo : MonoBehaviour
{
    public bool IsLegArm = false;
    public GameObject Parent = null;
    public List<GameObject> Children;

    void Start()
    {
        Children = new List<GameObject>();
    }
}
