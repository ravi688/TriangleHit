using UnityEngine;
using System.Collections;

public abstract class Shake : IManualUpdatable
{
    //Configure Vars
    public float MaxAmplitude;
    public float Phase;                             //Multiple of PI
    public float MaxFrequency;
    public float Duration;
    public Transform Target;
    public Vector2 Direction
    {
        get { return direction; }
        set
        {
            if (value.sqrMagnitude != 1.0)
                direction = value.normalized;
            else
                direction = value;
        }
    }
    public AnimationCurve frequencyVariationCurve;
    public AnimationCurve amplitudeVariationCurve;


    public bool IsRunning { get { return isShaking; } }
    public abstract void Update();
    public void shake()
    {
        initial_time = Time.time;
        isShaking = true;
        OnStart();
    }

    protected float time_elapsed;
    protected float frequency;
    protected float amplitude;
    protected Vector2 direction;
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected void tick()
    {
        time_elapsed = Time.time - initial_time;
        float normalized_time = time_elapsed / Duration;
        frequency = frequencyVariationCurve.Evaluate(normalized_time) * MaxFrequency;
        amplitude = amplitudeVariationCurve.Evaluate(normalized_time) * MaxAmplitude;

        if (normalized_time > 1)
        {
            OnStop();
            isShaking = false;
        }
    }


    private bool isShaking;
    private float initial_time;

}

public class RotationalShake : Shake
{
    public override void Update()
    {
        base.tick();
    }
    protected override void OnStop()
    {

    }
    protected override void OnStart()
    {
        throw new System.NotImplementedException();
    }
}

public class TranslationalShake : Shake
{
    private Vector3 relative_point;
  
  
    public TranslationalShake() { }
    public TranslationalShake(Transform target)
    {
        this.Target = target;
    }
    public override void Update()
    {
        base.tick();
        Vector3 shake_offset = Mathf.Sin(time_elapsed * frequency + Phase * Mathf.PI) * amplitude * direction;
        Target.position +=  shake_offset;  
    }
    protected override void OnStart()
    {
      //  relative_point = Target.position;
    }
    protected override void OnStop()
    {
       // Target.position = relative_point; 
    }
}