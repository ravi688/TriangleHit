#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] Wards;
    [SerializeField]
    private Transform GameCameraTransform;
    private Vector3 temp_pos;
    private float EachWardXSize;
    private float EachWardYSize;
    private int WardCount;
    private int DynamicElementCount;
    private bool isCullWards;

    private Vector2[] ward_positions;
    private Transform[] dynamic_element_containers;
    private Bounds[] ward_bounds;
    private List<Transform> dynamic_elements;

    private void Awake()
    {
        WardCount = Wards.Length;
        SpriteRenderer rend = Wards[0].GetComponent<SpriteRenderer>();
        EachWardXSize = rend.sprite.bounds.size.x * Wards[0].transform.localScale.x;
        EachWardYSize = rend.sprite.bounds.size.y * Wards[0].transform.localScale.y;
        isCullWards = true;

        AllocateMemory();
        Initialize();
    }
    private void SetWardFor(Transform element, int element_id)
    {
        for (int ward_id = 0; ward_id < WardCount; ward_id++)
            if (!element.IsChildOf(dynamic_element_containers[ward_id]) && IsInBounds(element.position, ward_positions[ward_id], ward_bounds[ward_id]))
            {
                if (dynamic_element_containers[ward_id].gameObject.activeInHierarchy)
                    element.SetParent(dynamic_element_containers[ward_id]);
            }
    }
    private void Initialize()
    {
        for (int i = 0; i < WardCount; i++)
        {
            Transform container = Wards[i].transform.Find("DynamicElements");
            int count = container.childCount;
            for (int j = 0; j < count; j++, DynamicElementCount++)
                dynamic_elements.Add(container.GetChild(j));

            ward_positions[i] = Wards[i].transform.position;
            ward_bounds[i] = Wards[i].GetComponent<SpriteRenderer>().bounds;
            dynamic_element_containers[i] = container;
        }
    }
    private void AllocateMemory()
    {
        dynamic_elements = new List<Transform>();
        ward_positions = new Vector2[WardCount];
        dynamic_element_containers = new Transform[WardCount];
        ward_bounds = new Bounds[WardCount];
    }

    private void Update()
    {
        if (isCullWards)
            CullInactiveWards();

        if (Input.GetKeyDown(KeyCode.U))
            for (int element_id = 0; element_id < DynamicElementCount; element_id++)
            {
                Debug.Log(dynamic_elements[element_id].name);
                SetWardFor(dynamic_elements[element_id], element_id);
            }
        for (int i = 0; i < DynamicElementCount; i++)
            SetWardFor(dynamic_elements[i], i); 
    }

    private bool IsInBounds(Vector2 point, Vector2 center, Bounds bounds)
    {
        Vector2 top_right_corner = (Vector2)bounds.extents + center;
        Vector2 bottom_left_corner = -(Vector2)bounds.extents + center;
        if (point.x > bottom_left_corner.x && point.x < top_right_corner.x && point.y < top_right_corner.y && point.y > bottom_left_corner.y)
            return true;
        return false;
    }
    private void CullInactiveWards()
    {
        for (int i = 0; i < WardCount; i++)
        {

            temp_pos = Wards[i].transform.position;
            bool IsLROfCameraBounds = Mathf.Abs(GameCameraTransform.position.x - temp_pos.x) > EachWardXSize;
            bool IsTBOfCameraBounds = Mathf.Abs(GameCameraTransform.position.y - temp_pos.y) > EachWardYSize;

            if ((IsLROfCameraBounds || IsTBOfCameraBounds) && Wards[i].activeSelf)
            {
                Wards[i].SetActive(false);
            }
            else
                if (!(IsLROfCameraBounds || IsTBOfCameraBounds) && !Wards[i].activeSelf)
                {
                    Wards[i].SetActive(true);
                }
        }
    }
}
