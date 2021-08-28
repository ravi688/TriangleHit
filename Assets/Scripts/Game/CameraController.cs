
#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Header("Follower Configuration")]
    [SerializeField]
    private float followSpeed = 2.0f;
    [SerializeField]
    private float xClampOffset;
    [SerializeField]
    private float yClampOffset;
    [SerializeField]
    private GameObject top_left_ward;
    [SerializeField]
    private GameObject top_right_ward;
    [SerializeField]
    private GameObject bottom_left_ward;
    [SerializeField]
    private GameObject bottom_right_ward;

    [Space]
    [Header("Shake Configuration")]
    [SerializeField]
    private AnimationCurve frequencyCurve;
    [SerializeField]
    private AnimationCurve amplitudeCurve;
    [SerializeField]
    private float Amplitude = 1;
    [SerializeField]
    private float frequency = 10;
    [SerializeField]
    private float shake_duration = 1;
    [SerializeField]
    private Vector2 direction = Vector2.left;
    [Space]
    [Header("ZoomIn and Out Config")]
    [SerializeField]
    private AnimationCurve ZoomInCurve;
    [SerializeField]
    private AnimationCurve ZoomOutCurve;
    [SerializeField]
    private float maxOrthographicSizeOffset = 10.0f;
    [SerializeField]
    private float zoom_duration = 1.0f;

    private Camera cam;

    private float initial_orthographicSize;
    private float timing;
    private bool isZoomIn;
    private bool isZoomOut;
    private float normalized_time;

    [HideInInspector] public TranslationalShake ShakeContoller;

    private Transform player_transform
#if !RELEASE_MODE
    {
        get { return __palyer_transform;  }
        set
        {
            if (value == null)
                GameManager.GetLogManager().Log("NULL");
            __palyer_transform = value;
        }
    }
    private Transform __palyer_transform;
#else
        ;
#endif
    private bool IsFollow;

    private float max_y;
    private float min_y;
    private float max_x;
    private float min_x;
    private float half_movable_region_width;
    private float half_movable_region_height;
    private float inverse_half_movable_region_width;
    private float inverse_half_movable_region_height;

    private bool Is_right_clamp;
    private bool Is_left_clamp;
    private bool Is_top_clamp;
    private bool Is_bottom_clamp;

    private float x_pos;
    private float y_pos;

    private bool ward_assigned = false;
    Vector2 world_screen_size;


    private float inverse_zoom_duration;

    private void Start()
    {
        IsFollow = false;
        inverse_zoom_duration = (float)1 / zoom_duration;

        if ((top_left_ward == null) || (top_right_ward == null) || (bottom_left_ward == null) || (bottom_right_ward == null))
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Either of TOP LEFT, TOP RIGHT, BOTTOM LEFT, BOTTOM RIGHT Ward is not assigned");
#endif
            half_movable_region_height = 1000;
            half_movable_region_width = 1000;
            max_y = 1000;
            min_y = -1000;
            max_x = 1000;
            min_x = -1000;
        }
        else
        {
            ward_assigned = true;
            
            Bounds top_left_bounds = top_left_ward.GetComponent<SpriteRenderer>().bounds;
            Bounds top_right_bounds = top_right_ward.GetComponent<SpriteRenderer>().bounds;
            Bounds bottom_left_bounds = bottom_left_ward.GetComponent<SpriteRenderer>().bounds;
            Bounds bottom_right_bounds = bottom_right_ward.GetComponent<SpriteRenderer>().bounds;
            Camera camera = GameManager.GetCamera();
            world_screen_size = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            float horizontal_offset = world_screen_size.x * 0.5f;
            float vertical_offset = world_screen_size.y * 0.5f;
            min_y = bottom_left_bounds.min.y + vertical_offset;
            max_y = top_left_bounds.max.y - vertical_offset;
            min_x = bottom_left_bounds.min.x + horizontal_offset;
            max_x = bottom_right_bounds.max.x- horizontal_offset;
            half_movable_region_height = (max_y - min_y) * 0.5f;
            half_movable_region_width = (max_x - min_x) * 0.5f;
            inverse_half_movable_region_width = (float)1 / half_movable_region_width;
            inverse_half_movable_region_height = (float)1 / half_movable_region_height;
        }

        max_y -= yClampOffset;
        min_y += yClampOffset;
        max_x -= xClampOffset;
        min_x += xClampOffset;

        ShakeContoller = new TranslationalShake(this.transform);
        ShakeContoller.frequencyVariationCurve = frequencyCurve;
        ShakeContoller.amplitudeVariationCurve = amplitudeCurve;
        ShakeContoller.Phase = 0;
        ShakeContoller.Direction = direction;
        ShakeContoller.MaxAmplitude = Amplitude;
        ShakeContoller.MaxFrequency = frequency;
        ShakeContoller.Duration = shake_duration;

        cam = GetComponent<Camera>();
        initial_orthographicSize = cam.orthographicSize;
        isZoomIn = false;
        isZoomOut = false; 
    }

    //private void OnDrawGizmos()
    //{
    //    CDebug.DrawRectWithCornerPoints(new Vector3(-world_screen_size.x * 0.5f, world_screen_size.y * 0.5f) + transform.position,
    //                                    new Vector3(world_screen_size.x * 0.5f, world_screen_size.y * 0.5f) + transform.position,
    //                                    new Vector3(world_screen_size.x * 0.5f, -world_screen_size.y * 0.5f) + transform.position,
    //                                    new Vector3(-world_screen_size.x * 0.5f, -world_screen_size.y * 0.5f) + transform.position, Color.white);
    //    CDebug.DrawRectWithCornerPoints(new Vector2(min_x, max_y), new Vector2(max_x, max_y), new Vector2(max_x, min_y), new Vector2(min_x, min_y), Color.white);
    //}
    public void StopFollow()
    {
        IsFollow = false;
    }
    //called by external interrupt/script to trigger the following
    public void Follow(Transform target)
    {
        player_transform = target;
        IsFollow = true;
    }

    private void LateUpdate()
    {
        if (IsFollow)
        {
            FollowPlayer();
        }
        if (ShakeContoller.IsRunning)
            ShakeContoller.Update();

        HandleZoom();
    }

    private void HandleZoom()
    {
        if ((isZoomIn || isZoomOut) && normalized_time >= 1.0f)
        {
            isZoomIn = false;
            isZoomOut = false;
        }

        if (isZoomIn)
        {
            normalized_time = (Time.time - timing) * inverse_zoom_duration;
            cam.orthographicSize = initial_orthographicSize + ZoomInCurve.Evaluate(normalized_time) * maxOrthographicSizeOffset;
        }
        if (isZoomOut)
        {
            normalized_time = (Time.time - timing) * inverse_zoom_duration;
            cam.orthographicSize = -maxOrthographicSizeOffset * ZoomOutCurve.Evaluate(normalized_time) + initial_orthographicSize;
        }
    }
    public void Shake(Vector2 dir)
    {
        ShakeContoller.Direction = dir;
        ShakeContoller.shake();
    }
    private void FollowPlayer()
    {
        if (!ward_assigned)
        {
            Is_bottom_clamp = Is_top_clamp = Is_left_clamp = Is_right_clamp = Is_top_clamp = false;
        }
        else
        {
            Is_bottom_clamp = player_transform.position.y < (min_y + half_movable_region_height);
            Is_top_clamp = player_transform.position.y > (max_y - half_movable_region_height);
            Is_left_clamp = player_transform.position.x < (min_x + half_movable_region_width);
            Is_right_clamp = player_transform.position.x > (max_x - half_movable_region_width);
        }

        if (Is_right_clamp)
            x_pos = Mathf.Lerp(max_x, max_x - half_movable_region_width, Mathf.Clamp01((max_x- player_transform.position.x) *  inverse_half_movable_region_width));
        else if (Is_left_clamp)
            x_pos = Mathf.Lerp(min_x, min_x + half_movable_region_width, Mathf.Clamp01((player_transform.position.x - min_x) *  inverse_half_movable_region_width));
        else
            x_pos = player_transform.position.x;

        if (Is_top_clamp)
            y_pos = Mathf.Lerp(max_y, max_y - half_movable_region_height, Mathf.Clamp01((max_y - player_transform.position.y) * inverse_half_movable_region_height));
        else if (Is_bottom_clamp)
            y_pos = Mathf.Lerp(min_y, min_y + half_movable_region_height, Mathf.Clamp01((player_transform.position.y - min_y) * inverse_half_movable_region_height));
        else
            y_pos = player_transform.position.y;

        transform.position = Vector3.Lerp(transform.position, new Vector3(x_pos, y_pos, -10), Time.deltaTime * followSpeed);
    }
    public void ZoomIn()
    {
        isZoomIn = true;
        timing = Time.time;
        normalized_time = 0;
        initial_orthographicSize = cam.orthographicSize;
    }
    public void ZoomOut()
    {
        isZoomOut = true;
        timing = Time.time;
        normalized_time = 0;
        initial_orthographicSize = cam.orthographicSize;
    }
}
