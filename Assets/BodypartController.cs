using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodypartController : MonoBehaviour
{
    public GameObject[] PivotPoints;
    public float ForceMultiplier = 1.0f;
    private GameObject _current_pivot;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rb;
    private float _force_timeout;

    private GameObject GetCurrentPivot()
    {
        if (PivotPoints.Length == 0)
        {
            return null;
        }

        if (PivotPoints.Length == 1)
        {
            return PivotPoints[0];
        }

        GameObject best = PivotPoints[0];
        
        for (int i = 1; i < PivotPoints.Length; ++i)
        {
            var p = PivotPoints[i];
            var y_diff = Mathf.Abs(p.transform.position.y - best.transform.position.y);

            if (y_diff < 0.5f)
            {
                if (p.transform.position.x > best.transform.position.x)
                    best = p;
            }
            else
            {
                if (p.transform.position.y > best.transform.position.y)
                    best = p;
            }
        }
        
        return best;
    }

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _current_pivot = PivotPoints[0];
        _force_timeout = Random.Range(0.5f, 3.0f);
    }

    void Update()
    {
        _force_timeout -= Time.deltaTime;

        if (_force_timeout >= 0)
        {
            return;
        }

        var pivot = GetCurrentPivot();

        if (pivot == null)
        {
            if (PivotPoints.Length == 0)
            {
                transform.Rotate(0, 0, Time.deltaTime * 100.0f);
                return;
            }
            else
            {
                return;
            }
        }
            
        if (_force_timeout < 4)
            transform.Rotate(0, 0, Time.deltaTime);

        const float TIMEOUT_FORCE = 1.0f;

        var upper_mult = 3.0f;
        if (gameObject.layer == 9)
            upper_mult = 1.2f;

        _force_timeout = Random.Range(TIMEOUT_FORCE*0.8f, TIMEOUT_FORCE*upper_mult);

        var force_dir_obj = pivot.transform.GetChild(0);
        var force_dir = (force_dir_obj.transform.position - pivot.transform.position).normalized * 2.6f * ForceMultiplier;
        _rb.AddForceAtPosition(force_dir, pivot.transform.position, ForceMode2D.Impulse);
        //var current_pivot = GetCurrentPivot();
        //_renderer.sprite = Sprite.Create(_renderer.sprite.texture, _renderer.sprite.rect, current_pivot);
        //transform.Rotate(0, 0, Time.deltaTime);
    }
}
