
using UnityEditor;
using UnityEngine;


public class DuplicateAdjacentUtility : DuplicateUtility
{
    [MenuItem("Utility/Duplicate/Adjacent")]
    public static void ShowDuplicateAdjacentUtility()
    {
        Show<DuplicateAdjacentUtility>("Duplicate Adjacent Utility");
    }

    int duplicate_number = 1;
    bool x_axis = true;
    bool x_reversed = false;
    bool y_axis = false;
    bool y_reversed = false;
    bool y_flip = false;
    bool x_flip = false;


    protected override void OnDisplay()
    {
        EditorGUILayout.PrefixLabel("Duplicate Count");
        duplicate_number = EditorGUILayout.IntField(duplicate_number);
        x_axis = EditorGUILayout.ToggleLeft("X Axis", x_axis);
        x_reversed = EditorGUILayout.ToggleLeft("X Reversed", x_reversed);
        x_flip = EditorGUILayout.ToggleLeft("X Flip", x_flip);

        y_axis = EditorGUILayout.ToggleLeft("Y Axis", y_axis);
        y_reversed = EditorGUILayout.ToggleLeft("Y Reversed", y_reversed);
        y_flip = EditorGUILayout.ToggleLeft("Y Flip", y_flip);
    }
    protected override void OnDuplicate()
    {
        DuplicateSprites();
    }

    private void DuplicateSprites()
    {
        InputOutputContainer inputOutput = new InputOutputContainer();
        inputOutput.in_duplicate_number = duplicate_number;
        inputOutput.in_x_axis = x_axis;
        inputOutput.in_x_reversed = x_reversed;
        inputOutput.in_x_flip = x_flip;
        inputOutput.in_y_axis = y_axis;
        inputOutput.in_y_reversed = y_reversed;
        inputOutput.in_y_flip = y_flip;
        inputOutput.out_total_duplications = 0;
        DuplicateUtility.ForeachSelectedGameObject<InputOutputContainer>(duplicate, inputOutput);
    }
    struct InputOutputContainer
    {
        public int in_duplicate_number;
        public bool in_x_axis;
        public bool in_x_reversed;
        public bool in_x_flip;
        public bool in_y_axis;
        public bool in_y_reversed;
        public bool in_y_flip;
        public int out_total_duplications;
    }
    private void duplicate(GameObject __object, InputOutputContainer args)
    {
        SpriteRenderer renderer = __object.GetComponent<SpriteRenderer>();
        if (renderer == null)
            return;
        Bounds bounds = renderer.bounds;
        Vector3 origin = __object.transform.position;

        int duplicate_number = args.in_duplicate_number;
        int x_sign = args.in_x_reversed ? -1 : 1;
        int y_sign = args.in_y_reversed ? -1 : 1;
        bool x_flip = args.in_x_flip;
        bool y_flip = args.in_y_flip;
        int total_duplicate_count = 0;
        //duplicate in x direction
        if (args.in_x_axis)
            for (int i = 0; i < duplicate_number; i++, total_duplicate_count++)
            {
                GameObject duplicate_object = Instantiate<GameObject>(__object);
                Undo.RegisterCreatedObjectUndo(duplicate_object, duplicate_object.name);
                duplicate_object.transform.position = Vector3.right * x_sign * (i + 1) * bounds.extents.x * 2 + origin;
                duplicate_object.GetComponent<SpriteRenderer>().flipX = x_flip;
                x_flip = !x_flip;
            }
        //duplicate in y direction
        if (args.in_y_axis)
            for (int i = 0; i < duplicate_number; i++, total_duplicate_count++)
            {
                GameObject duplicate_object = Instantiate<GameObject>(__object);
                Undo.RegisterCreatedObjectUndo(duplicate_object, duplicate_object.name);
                duplicate_object.transform.position = Vector3.up * y_sign * (i + 1) * bounds.extents.y * 2 + origin;
                duplicate_object.GetComponent<SpriteRenderer>().flipY = y_flip;
                y_flip = !y_flip;
            }
    }
}
