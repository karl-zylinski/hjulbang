using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public Sprite[] Backgrounds;
    public int Order = 0;
    private GameObject _current_front;
    public float YOffset = 0;
    public float XOverlap = 0;
    public float XOffset = 0;
    private int _num_spawned;
    public bool SpawnPlants = false;
    public GameObject LastScreen;
    public bool Done = false;
    public Sprite PureGround;

    private Sprite GetNewBackground()
    {
        return Backgrounds[Random.Range(0, Backgrounds.Length - 1)];
    }

    private Vector2 GetNewFrontPos(Sprite new_sprite)
    {
        if (_current_front == null)
        {
            return new Vector2(XOffset + 20.0f, YOffset);
        }

        var x = XOverlap + _current_front.transform.position.x - _current_front.GetComponent<SpriteRenderer>().bounds.size.x / 2 - new_sprite.bounds.size.x / 2;
        return new Vector2(x, YOffset);
    }

    private void CreateNewFront(Sprite sprite)
    {
        var new_pos = GetNewFrontPos(sprite);
        var bg = new GameObject("bg");
        var sr = bg.AddComponent<SpriteRenderer>();
        bg.transform.parent = transform;
        sr.sprite = sprite;
        sr.sortingOrder = Order;
        bg.transform.position = new_pos;

        if (SpawnPlants && _num_spawned > 1)
        {
            var spawner = GameObject.FindWithTag("PlantSpawner");
            var num_to_spawn = Random.Range(2, 4);

            for (int i = 0; i < num_to_spawn; ++i)
                spawner.GetComponent<PlantSpawner>().NewScreenAdded(sr.bounds.min.x, sr.bounds.max.x);
        }

        ++_num_spawned;
        _current_front = bg;
    }

    void Start()
    {
        _num_spawned = 0;
        Done = false;
        CreateNewFront(GetNewBackground());
        CreateNewFront(GetNewBackground());
    }

    void Update()
    {
        if (Done == true)
            return;

        var attachpoints = GameObject.FindGameObjectsWithTag("AttachPoint");

        if (attachpoints.Length < 2)
        {
            if (LastScreen == null)
                return;

            /*for (int i = 0; i < 2; ++i)
            {
                var pos_x = _current_front.transform.position.x - _current_front.GetComponent<SpriteRenderer>().bounds.size.x / 2 - PureGround.bounds.size.x / 2;
                var new_pos = new Vector2(pos_x, -3.0f);
                var bg = new GameObject("bg");
                var sr = bg.AddComponent<SpriteRenderer>();
                bg.transform.parent = transform;
                sr.sprite = PureGround;
                sr.sortingOrder = -4;
                bg.transform.position = new_pos;
                ++_num_spawned;
                _current_front = bg;
                GameObject.Find("CollisionBoxLooper").GetComponent<CollisionBoxLooper>().CreateNewFront();
            }*/

            var obj = Instantiate(LastScreen);
            var srs = obj.GetComponentsInChildren<SpriteRenderer>();

            var min_x = srs[0].bounds.min.x;
            var max_x = srs[0].bounds.max.x;

            foreach(var sr in srs)
            {
                if (sr.bounds.min.x < min_x)
                    min_x = sr.bounds.min.x;

                if (sr.bounds.max.x > max_x)
                    max_x = sr.bounds.max.x;
            }

            var size = max_x - min_x;
            var x = _current_front.transform.position.x - _current_front.GetComponent<SpriteRenderer>().bounds.size.x / 2 - size / 2;
            var pos = new Vector2(x, 0.96f);
            obj.transform.position = pos;
            GameObject.Find("CollisionBoxLooper").GetComponent<CollisionBoxLooper>().Done = true;
            var bls = FindObjectsOfType<BackgroundLooper>();
            foreach(var bl in bls)
            {
                bl.Done = true;
            }
            return;
        }

        Vector2 cp = Camera.main.transform.position;
        var front_metabody = Camera.main.GetComponent<CameraControl>().MetabodiesByXCoord[0];

        if (front_metabody.Count == 0)
            return;

        var front_body = front_metabody[0];

        while (true)
        {
            var cfb = _current_front.GetComponent<SpriteRenderer>().bounds;
            var dist = cfb.min.x - (front_body.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect);
            if (Mathf.Abs(dist) < 1)
            {
                CreateNewFront(GetNewBackground());
            }
            else
                break;
        }
    }
}

