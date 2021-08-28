#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScreenFitter : MonoBehaviour
{

    [SerializeField]
    private Camera main_camera;
    [SerializeField]
    private Vector2 border_offset = new Vector2(0.5f, 0.5f);
    // [SerializeField]
    // private Vector2 reference_screen_size = new Vector2(1280, 800); 
    void Awake()
    {
        FitMethod1();
        //FitMethod2(); 
    }
    //private void FitMethod2()
    //{
    //    Vector2 world_screen_size = main_camera.ScreenToWorldPoint(reference_screen_size); 
    //    float aspect_ratio = (float)reference_screen_size.x / (float)reference_screen_size.y ;
    //    float reference_orthographic_size = world_screen_size.x / aspect_ratio;
    //    float current_orthographic_size = main_camera.orthographicSize;

    //    float scale_factor = current_orthographic_size / reference_orthographic_size;
    //    Debug.Log(scale_factor); 
    //}
    private void FitMethod1()
    {
        Vector2 orthographic_size = new Vector3(2 * (float)Screen.width / Screen.height * main_camera.orthographicSize,
            main_camera.orthographicSize * 2);
        Vector2 new_tv_screen_size = orthographic_size - border_offset;


        Bounds sprite_bounds = GetComponent<SpriteRenderer>().sprite.bounds;

        Vector2 old_tv_screen_size = new Vector2(sprite_bounds.size.x * transform.localScale.x, sprite_bounds.size.y * transform.localScale.y);
        float x_scale_factor = new_tv_screen_size.x / old_tv_screen_size.x;
        float y_scale_factor = new_tv_screen_size.y / old_tv_screen_size.y;

        transform.localScale = new Vector3(x_scale_factor, y_scale_factor, 1.0f);
    }
}
