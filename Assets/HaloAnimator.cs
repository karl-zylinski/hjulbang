using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloAnimator : MonoBehaviour
{
    public enum HaloState
    {
        Normal,
        GoingUp,
        GoingToBody,
        Done
    };

    public Sprite[] SparkleSprites;
    public int NumSparkles = 20;
    private GameObject[] _sparkles;
    private Vector2[] _targets;
    private Vector2[] _velocities;
    private float[] _initial_distance;
    private float[] _dist_traveled;
    public HaloState CurrentHaloState;
    private float _fly_up_cooldown;
    private GameObject _fly_target;

    private int GetNewRandomSortOrder()
    {
        var parent_sr = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        return Random.Range(0, 2) == 0 ? parent_sr.sortingOrder + 1 : parent_sr.sortingOrder - 1;
    }

    void Start()
    {
        _sparkles = new GameObject[NumSparkles];
        _targets = new Vector2[NumSparkles];
        _velocities = new Vector2[NumSparkles];
        _initial_distance = new float[NumSparkles];
        _dist_traveled = new float[NumSparkles];
        CurrentHaloState = HaloState.Normal;
        var parent_sr = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        var parent_sort_order = parent_sr.sortingOrder;
        var parent_bounds = parent_sr.bounds;
        var bc = GetComponent<BoxCollider2D>();

        for (int i = 0; i < NumSparkles; ++i)
        {
            var go = new GameObject();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SparkleSprites[Random.Range(0, SparkleSprites.Length - 1)];
            sr.sortingOrder = GetNewRandomSortOrder();
            go.transform.parent = gameObject.transform;
            _sparkles[i] = go;
            _targets[i] = new Vector2(Random.Range(bc.bounds.min.x, bc.bounds.max.x),
                                     Random.Range(bc.bounds.min.y, bc.bounds.max.y));
            var start = new Vector2(Random.Range(bc.bounds.min.x, bc.bounds.max.x),
                                     Random.Range(bc.bounds.min.y, bc.bounds.max.y));
            go.transform.position = start;
            _velocities[i] = (_targets[i] - start) * Random.Range(0, 0.5f);
            _initial_distance[i] = (_targets[i] - start).magnitude;
            _dist_traveled[i] = 0;
        }
    }

    public void EnterGoingToBody(GameObject part)
    {
        var bpi = part.GetComponent<BodypartInfo>();
        var metabody = bpi.MetaBody;
        CurrentHaloState = HaloState.GoingUp;
        _fly_up_cooldown = 1.3f;
        _fly_target = part;

        for (int i = 0; i < NumSparkles; ++i)
        {
           _targets[i] = transform.position + new Vector3(0, 3.0f, 0);
        }
    }


    void Update()
    {
        if (CurrentHaloState == HaloState.GoingUp)
        {
            _fly_up_cooldown -= Time.deltaTime;
            var glow = transform.Find("HaloGlow").gameObject;

            var ns = glow.transform.localScale - new Vector3(Time.deltaTime, Time.deltaTime * 0.2f, 0);

            if (ns.x < 0)
                ns.x = 0;

            if (ns.y < 0)
                ns.y = 0;

            glow.transform.localScale = ns;

            if (_fly_up_cooldown <= 0)
            {
                CurrentHaloState = HaloState.GoingToBody;
            }
        }

        var parent_sr = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        var bc = GetComponent<BoxCollider2D>();
        var bcb_min = bc.bounds.min;
        var bcb_max = bc.bounds.max;
        var all_invisible = true;
        for (int i = 0; i < NumSparkles; ++i)
        {
            var s = _sparkles[i];
            var sr = s.GetComponent<SpriteRenderer>();
            Vector2 p = s.transform.position;
            if (CurrentHaloState == HaloState.Normal)
            {
                _velocities[i] -= _velocities[i] * 0.99f * Time.deltaTime;
                _velocities[i] += (_targets[i] - p) * Time.deltaTime;
            }
            else if (CurrentHaloState == HaloState.GoingUp)
            {
                _velocities[i] += (_targets[i] - p) * 13.0f * Time.deltaTime;
            }
            else if (CurrentHaloState == HaloState.GoingToBody)
            {
                Vector2 t2 = _fly_target.transform.position;
                _velocities[i] = (t2 - p).normalized * 14.0f;
            }

            var diff = _velocities[i] * Time.deltaTime;
            _dist_traveled[i] += diff.magnitude;
            s.transform.position = p + diff;
            Debug.DrawLine(s.transform.position, _targets[i]);

            if (CurrentHaloState == HaloState.Normal && _dist_traveled[i] >= _initial_distance[i])
            {
                Vector2 start = s.transform.position;
                _targets[i] = new Vector2(Random.Range(bcb_min.x, bcb_max.x),
                                          Random.Range(bcb_min.y, bcb_max.y));

                if (Mathf.Abs(bcb_min.x - start.x) < 0.4
                    || Mathf.Abs(bcb_min.y - start.y) < 0.4
                    || Mathf.Abs(bcb_max.x - start.x) < 0.4
                    || Mathf.Abs(bcb_max.y - start.y) < 0.4)
                {
                    sr.sortingOrder = sr.sortingOrder == 4 ? 6 : 4;
                }

                _dist_traveled[i] = 0;
                _initial_distance[i] = (_targets[i] - start).magnitude;
            }
            else if (CurrentHaloState == HaloState.GoingToBody)
            {
                Vector2 t2 = _fly_target.transform.position;
                if ((t2 - p).magnitude < 0.3)
                    sr.enabled = false;
            }
            
            if (sr.enabled)
                all_invisible = false;
        }

        if (all_invisible)
        {
            CurrentHaloState = HaloState.Done;
            Destroy(gameObject);
        }
    }
}
