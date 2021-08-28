using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    private Vector2 scale;
    private Sprite sprite; 
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>().sprite; 
    }
    void Start()
    {
        Vector2 world_view_size = new Vector2(Camera.main.orthographicSize * 2 * Screen.width  / Screen.height , Camera.main.orthographicSize * 2);
        Vector2 world_sprite_size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
        transform.localScale = new Vector3(world_view_size.x / world_sprite_size.x, world_view_size.y / world_sprite_size.y , 1);
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,
            transform.position.z); 
    }
}
