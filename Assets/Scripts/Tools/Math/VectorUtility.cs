
using UnityEngine;
public static class VectorUtility
{
    public static readonly Vector3 null_vector3 = Vector3.zero;
    public static readonly Vector2 null_vector2 = Vector2.zero;
    public static void Multiply(Vector3 vector1, Vector3 vector2, ref Vector3 out_product)
    {
        out_product.x = vector1.x * vector2.x;
        out_product.y = vector1.y * vector2.y;
        out_product.z = vector1.z * vector2.z; 
    }
    public static void Divide(Vector3 nominator, Vector3 denominator, ref Vector3 out_quotient)
    {
        if ((denominator.x * denominator.y * denominator.z) == 0)
        {
            Debug.LogError("Denominator vector has a zero component"); 
            return; 
        }
        out_quotient.x = nominator.x / denominator.x;
        out_quotient.y = nominator.y / denominator.y;
        out_quotient.z = nominator.z / denominator.z; 
    }
    public static float GetDistance(Vector2 point1, Vector2 point2)
    {
        Vector2 displacement_vector = point1 - point2;
        Vector2 _aux_vector = Rotate(displacement_vector,GetSignedAngle(Vector2.right, displacement_vector));
        return _aux_vector.x < 0 ? -_aux_vector.x : _aux_vector.x;
    }
    public static float GetSignedAngle(Vector2 from, Vector2 to, bool isAntiClwPstv = true)
    {
        if (to == Vector2.zero)
            to = Vector2.right;

        float angle = Vector2.Angle(from, to);
        Vector3 _crp_ = Vector3.Cross(from, to);
        angle *= Mathf.Sign(Vector3.Dot(Vector3.forward, _crp_)) * (isAntiClwPstv ? 1 : -1);
        return angle;
    }

    //Angles in degrees
    public static Vector2 Rotate(Vector2 vector, float angle)      //+ve for anticlockwise , -ve for clockwise
    {
        angle *= Mathf.Deg2Rad;
        float _sin_value = Mathf.Sin(angle);
        float _cos_value = Mathf.Cos(angle);
        return new Vector2(_cos_value * vector.x + _sin_value * vector.y, -_sin_value * vector.x + _cos_value * vector.y);
    }
}