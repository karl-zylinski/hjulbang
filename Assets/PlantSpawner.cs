using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    public GameObject PlantPrefab;
    public Sprite[] PlantSprites;
    
    public void NewScreenAdded(float x_start, float x_end)
    {
        var x = Random.Range(x_start, x_end);
        var plant = Instantiate(PlantPrefab);
        var sr = plant.GetComponent<SpriteRenderer>();
        sr.sprite = PlantSprites[Random.Range(0, PlantSprites.Length - 1)];
        sr.transform.position = new Vector2(x, -1.7f);
    }
}
