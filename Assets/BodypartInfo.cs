using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartInfo : MonoBehaviour
{
    public bool IsLegArm = false;
    public List<GameObject> MetaBody;

    void Start()
    {
        MetaBody = new List<GameObject>();
        MetaBody.Add(gameObject);
    }

    public static List<List<GameObject>> GetAllMetabodies()
    {
        var metabodies = new List<List<GameObject>>();
        var infos = FindObjectsOfType<BodypartInfo>();
        foreach (var info in infos)
        {
            if (!metabodies.Contains(info.MetaBody))
                metabodies.Add(info.MetaBody);
        }
        return metabodies;
    }
}
