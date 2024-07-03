using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuaternion : MonoBehaviour
{
    #region PROPIEDADES

    public float x;
    public float y;
    public float z;
    public float w;

    private static readonly MyQuaternion identityQuaternion = new MyQuaternion(0f, 0f, 0f, 1f); //Quaternion de identidad, no rotacion

    public const float kEpsilon = 1E-06f;

    #endregion

    #region ACCESORS
    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw new IndexOutOfRangeException("Invalid Quaternion index!"),
            };
        }

        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Quaternion index!");
            }
        }
    }

    public MyQuaternion(float x, float y, float z, float w) //Constructor
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public void Set(float newX, float newY, float newZ, float newW) //Setea los valores de un quaternion existente
    {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
    }

    public static MyQuaternion identity //Obtiene el quaternion identidad
    {
        get
        {
            return identityQuaternion;
        }
    }
    
    public Vector3 eulerAngles //Returns or sets the euler angle representation of the rotation in degrees.
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    public MyQuaternion normalized //Returns this quaternion with a magnitude of 1.
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region FUNCIONES
    public static MyQuaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion Inverse(MyQuaternion rotation)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion Slerp(MyQuaternion a, MyQuaternion b, float t)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion Lerp(MyQuaternion a, MyQuaternion b, float t)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion AngleAxis(float angle, Vector3 axis)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion LookRotation(Vector3 forward, Vector3 upwards)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion LookRotation(Vector3 forward)
    {
        return LookRotation(forward, Vector3.up);
    }

    public static float Dot(MyQuaternion a, MyQuaternion b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public void SetLookRotation(Vector3 view, Vector3 up)
    {
        this.x = LookRotation(view, up).x;
        this.y = LookRotation(view, up).y;
        this.z = LookRotation(view, up).z;
        this.w = LookRotation(view, up).w;
    }

    public void SetLookRotation(Vector3 view)
    {
        Vector3 up = Vector3.up;
        SetLookRotation(view, up);
    }

    public static float Angle(MyQuaternion a, MyQuaternion b)
    {
        float num = Mathf.Min(Mathf.Abs(Dot(a, b)), 1f);
        return IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
    }

    public static MyQuaternion Euler(float x, float y, float z)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion Euler(Vector3 euler)
    {
        throw new NotImplementedException();
    }

    public void ToAngleAxis(out float angle, out Vector3 axis)
    {
        throw new NotImplementedException();
    }

    public static MyQuaternion RotateTowards(MyQuaternion from, MyQuaternion to, float maxDegreesDelta)
    {
        float num = Angle(from, to);
        if (num == 0f)
        {
            return to;
        }

        return SlerpUnclamped(from, to, Mathf.Min(1f, maxDegreesDelta / num));
    }

    public static MyQuaternion Normalize(MyQuaternion q)
    {
        float num = Mathf.Sqrt(Dot(q, q));
        if (num < Mathf.Epsilon)
        {
            return identity;
        }

        return new MyQuaternion(q.x / num, q.y / num, q.z / num, q.w / num);
    }

    public void Normalize()
    {
        this.x = Normalize(this).x;
        this.y = Normalize(this).y;
        this.z = Normalize(this).z;
        this.w = Normalize(this).w;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
    }

    public override bool Equals(object other)
    {
        if (!(other is MyQuaternion))
        {
            return false;
        }

        return Equals((MyQuaternion)other);
    }

    public bool Equals(MyQuaternion other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }


    #endregion

    #region OPERADORES
    public static MyQuaternion operator *(MyQuaternion lhs, MyQuaternion rhs)
    {
        return new MyQuaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
    }

    public static Vector3 operator *(MyQuaternion rotation, Vector3 point)
    {
        throw new NotImplementedException();
    }

    private static bool IsEqualUsingDot(float dot)
    {
        return dot > 0.999999f;
    }

    public static bool operator ==(MyQuaternion lhs, MyQuaternion rhs)
    {
        return IsEqualUsingDot(Dot(lhs, rhs));
    }

    public static bool operator !=(MyQuaternion lhs, MyQuaternion rhs)
    {
        return !(lhs == rhs);
    }

    #endregion
}
