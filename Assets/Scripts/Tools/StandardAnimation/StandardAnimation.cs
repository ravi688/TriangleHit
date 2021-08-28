using UnityEngine; 

[CreateAssetMenu]
public class StandardAnimation : ScriptableObject
{
    public Vector3 MaxScaleOffset;
    public Vector3 MaxPositionOffset;
    public Vector3 MaxRotationOffset; 
    public AnimationCurve YPositionOffsetOverTime;
    public AnimationCurve XPositionOffsetOverTime;
    public AnimationCurve ZPositionOffsetOverTime;
    public AnimationCurve YRotationOffsetOverTime;
    public AnimationCurve XRotationOffsetOverTime;
    public AnimationCurve ZRotationOffsetOverTime;
    public AnimationCurve XScaleOffsetOverTime;
    public AnimationCurve YScaleOffsetOverTime;
    public AnimationCurve ZScaleOffsetOverTime;
    public AnimationCurve AlphaOverTime;
    public bool IsLoop = false;
    public float AnimationDuration;
    public bool IsLocalSpace = true;
    public bool IsAnimateRotation = false;
    public bool IsAnimatePosition = true;
    public bool IsAnimationAlpha = false;
    public bool IsAnimateScale = false;

    public StandardAnimation()
    {
        YPositionOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        XPositionOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        ZPositionOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        YRotationOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        ZRotationOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        XRotationOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        XScaleOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        YScaleOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        ZScaleOffsetOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        AlphaOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    }


}
