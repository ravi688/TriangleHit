using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCreator : MonoBehaviour
{


    public GameObject bottom_HorizontalBorder;
    public GameObject top_HorizontalBorder;
    public GameObject left_VerticalBorder;
    public GameObject right_VerticalBorder;

    public GameObject bottom_HorizontalBorderCollider;
    public GameObject top_HorizontalBorderCollider;
    public GameObject left_VerticalBorderCollider;
    public GameObject right_VerticalBorderCollider;


    [SerializeField]
    private float offset = 0.1f;
    private Vector2 view_size;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        view_size = new Vector2(mainCamera.orthographicSize * 2.0f * Screen.width / Screen.height, mainCamera.orthographicSize * 2);

        left_pos = (Vector2)mainCamera.transform.position - new Vector2(view_size.x * 0.5f + offset, 0);
        right_pos = (Vector2)mainCamera.transform.position + new Vector2(view_size.x * 0.5f + offset, 0);
        top_pos = (Vector2)mainCamera.transform.position + new Vector2(0, view_size.y * 0.5f + offset);
        bottom_pos = (Vector2)mainCamera.transform.position - new Vector2(0, view_size.y * 0.5f + offset);

        bottom_HorizontalBorderCollider.transform.position = bottom_pos;
        top_HorizontalBorderCollider.transform.position = top_pos;
        left_VerticalBorderCollider.transform.position = left_pos;
        right_VerticalBorderCollider.transform.position = right_pos;

        PositionBorders();
        float x_scale_horizontal = view_size.x / top_HorizontalBorder.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float y_scale_vertical = view_size.y / right_VerticalBorder.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        top_HorizontalBorder.transform.localScale = 
            top_HorizontalBorderCollider.transform.localScale = new Vector3(x_scale_horizontal, 1.0f, 1.0f);
        bottom_HorizontalBorder.transform.localScale
            = bottom_HorizontalBorderCollider.transform.localScale  = new Vector3(x_scale_horizontal, 1.0f, 1.0f);
        left_VerticalBorder.transform.localScale
            = left_VerticalBorderCollider.transform.localScale  = new Vector3(1.0f, y_scale_vertical, 1.0f);
        right_VerticalBorder.transform.localScale
            = right_VerticalBorderCollider.transform.localScale = new Vector3(1.0f, y_scale_vertical, 1.0f);
    }

    Vector2 left_pos;
    Vector2 right_pos;
    Vector2 top_pos;
    Vector2 bottom_pos;
    Vector3 previousCamPos;
    private void LateUpdate()
    {
        // if (previousCamPos != mainCamera.transform.position) ;
        PositionBorders();
    }

    private void PositionBorders()
    {
        left_pos = (Vector2)mainCamera.transform.position - new Vector2(view_size.x * 0.5f + offset, 0);
        right_pos = (Vector2)mainCamera.transform.position + new Vector2(view_size.x * 0.5f + offset, 0);
        top_pos = (Vector2)mainCamera.transform.position + new Vector2(0, view_size.y * 0.5f + offset);
        bottom_pos = (Vector2)mainCamera.transform.position - new Vector2(0, view_size.y * 0.5f + offset);

        bottom_HorizontalBorder.transform.position = bottom_pos;
        top_HorizontalBorder.transform.position = top_pos;
        left_VerticalBorder.transform.position = left_pos;
        right_VerticalBorder.transform.position = right_pos;
    }
}
