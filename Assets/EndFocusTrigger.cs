using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFocusTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bodypart")
            return;

        Camera.main.GetComponent<CameraControl>().Focus(other.gameObject);
        Destroy(gameObject);
    }
}
