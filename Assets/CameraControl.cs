using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject _following;
    public List<List<GameObject>> MetabodiesByXCoord;
    private float _wanted_x;
    private int _follow_index;

    void Start()
    {
        UpdateMetabodiesByXCoord();
        _following = MetabodiesByXCoord[0][0];
    }

    public void Focus(GameObject bodypart)
    {
        var bpi = bodypart.GetComponent<BodypartInfo>();
        var i = MetabodiesByXCoord.FindIndex(x => x == bpi.MetaBody);

        if (i != -1)
            _follow_index = i;
    }

    private void UpdateMetabodiesByXCoord()
    {
        var all_metabodies = BodypartInfo.GetAllMetabodies();
        all_metabodies.Sort(
            delegate (List<GameObject> l1, List<GameObject> l2)
                { return l1[0].transform.position.x.CompareTo(l2[0].transform.position.x); }
        );
        MetabodiesByXCoord = all_metabodies;
    }

    void Update()
    {
        if (Time.time < 0.2f)
            return;

        UpdateMetabodiesByXCoord();
        _following = MetabodiesByXCoord[_follow_index][0];
        _wanted_x = _following.transform.position.x;

        Vector3 current_pos = Camera.main.transform.position;
        var diff = (_wanted_x - current_pos.x);
        var new_x = current_pos.x + diff * Time.unscaledDeltaTime * 10.0f;

        if (new_x > 0)
            new_x = 0;

        Camera.main.transform.position = new Vector3(new_x, 0, -10);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            _follow_index -= 1;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _follow_index += 1;

        _follow_index = Mathf.Clamp(_follow_index, 0, MetabodiesByXCoord.Count - 1);
    }
}
