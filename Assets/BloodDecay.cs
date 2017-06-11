using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDecay : MonoBehaviour
{
    private float _remove_cooldown;

    void Start()
    {
        _remove_cooldown = Random.Range(30.0f, 60.0f);
    }

    void Update()
    {
        _remove_cooldown -= Time.deltaTime;

        if (_remove_cooldown < 0)
            Destroy(gameObject);
    }
}
