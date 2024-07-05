using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuaternion : MonoBehaviour
{
    #region PROPIEDADES

    public float x;
    public float y;
    public float z; //x, y, z son las partes imaginarias
    public float w; //parte real

    private static readonly MyQuaternion identityQuaternion = new MyQuaternion(0f, 0f, 0f, 1f); //Quaternion de identidad, no rotacion. Rotacion alineada con los ejes

    public const float kEpsilon = 1E-06f;

    #endregion

    #region ACCESORS
    public float this[int index] //Obtener y setear valores al quat a partir de un index
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

    public float magnitude
    {
        get
        {
            return Mathf.Sqrt(x * x + y * y + z * z + w * w);
        }
    }
    
    public Vector3 eulerAngles 
    {
        get
        {
            float sqW = w * w;
            float sqX = x * x;
            float sqY = y * y;
            float sqZ = z * z;

            float unit = sqX + sqY + sqZ + sqW; //Chequea si el quat esta normalizado (da 1 si si, sino lo corrige)

            float test = x * w - y * z; //Obtiene el valor de x

            Vector3 anglesVector = new Vector3();

            if (test > 0.4999f * unit) //Singularidad en polo norte (cuando x se acerca a +90°)
            {
                anglesVector.y = 2f * Mathf.Atan2(y, x); //Calcula los valores para que los ejes no se superpongan
                anglesVector.x = Mathf.PI / 2;
                anglesVector.z = 0;

                return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
            }
            if (test < -0.4999f * unit) //Singularidad en polo sur (cuando x se acerca a -90°)
            {
                anglesVector.y = -2f * Mathf.Atan2(y, x);
                anglesVector.x = -Mathf.PI / 2;
                anglesVector.z = 0;

                return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
            }

            MyQuaternion orderQuat = new MyQuaternion(w, z, x, y);

            anglesVector.y = (float)Math.Atan2(2f * orderQuat.x * orderQuat.w + 2f * orderQuat.y * orderQuat.z, 1 - 2f * (orderQuat.z * orderQuat.z + orderQuat.y * orderQuat.y));
            anglesVector.x = (float)Math.Asin(2f * (orderQuat.x * orderQuat.z - orderQuat.w * orderQuat.y));
            anglesVector.z = (float)Math.Atan2(2f * orderQuat.x * orderQuat.y + 2f * orderQuat.z * orderQuat.w, 1 - 2f * (orderQuat.y * orderQuat.y + orderQuat.z * orderQuat.z));

            return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
        }

        set
        {
            //Cada coordenada de los Euler representa en ese eje que tan rotado esta el objeto 

            float xInRad = Mathf.Deg2Rad * value.x * 0.5f; //Lo paso a radianes para poder trabajar con seno y coseno
            float yInRad = Mathf.Deg2Rad * value.y * 0.5f;
            float zInRad = Mathf.Deg2Rad * value.z * 0.5f;

            //Teniendo en cuenta la fórmula para rotar en 3D Cos(a/2) + iSin(a/2) + jSin(a/2) + kSin(a/2) calcula la rotacion en cada uno de los ejes.
            MyQuaternion qx = new MyQuaternion(Mathf.Sin(xInRad), 0, 0, Mathf.Cos(xInRad));
            MyQuaternion qy = new MyQuaternion(0, Mathf.Sin(yInRad), 0, Mathf.Cos(yInRad));
            MyQuaternion qz = new MyQuaternion(0, 0, Mathf.Sin(zInRad), Mathf.Cos(zInRad));

            MyQuaternion result = qy * qx * qz; //Hago la multiplicación para aplicar la rotación. Es en ese orden por como maneja el orden unity.

            this.x = result.x;
            this.y = result.y;
            this.z = result.z;
            this.w = result.w;
        }
    }

    public MyQuaternion normalized //Devuelve este quaternion con magnitud 1.
    {
        get
        {
            if(magnitude < kEpsilon)
            {
                return identity;
            }

            return new MyQuaternion(x / magnitude, y / magnitude, z / magnitude, w / magnitude);
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

    private static float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;

        while (angle < 0f)
            angle += 360;

        return angle;
    }

    private static Vector3 NormalizeAngles(Vector3 angles)
    {
        Vector3 normalizedAngles = new Vector3();

        normalizedAngles.x = NormalizeAngle(angles.x);
        normalizedAngles.y = NormalizeAngle(angles.y);
        normalizedAngles.z = NormalizeAngle(angles.z);

        return normalizedAngles;
    }

    public static Vector3 QuatToEulerRad(MyQuaternion q1)
    {
        float sqW = q1.w * q1.w;
        float sqX = q1.x * q1.x;
        float sqY = q1.y * q1.y;
        float sqZ = q1.z * q1.z;

        float unit = sqX + sqY + sqZ + sqW; //Chequea si el quat esta normalizado (da 1 si si, sino lo corrige)

        float test = q1.x * q1.w - q1.y * q1.z; //Obtiene el valor de x

        Vector3 anglesVector = new Vector3();

        if (test > 0.4999f * unit) //Singularidad en polo norte (cuando x se acerca a +90°)
        {
            anglesVector.y = 2f * Mathf.Atan2(q1.y, q1.x); //Calcula los valores para que los ejes no se superpongan
            anglesVector.x = Mathf.PI / 2;
            anglesVector.z = 0;

            return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
        }
        if (test < -0.4999f * unit) //Singularidad en polo sur (cuando x se acerca a -90°)
        {
            anglesVector.y = -2f * Mathf.Atan2(q1.y, q1.x);
            anglesVector.x = -Mathf.PI / 2;
            anglesVector.z = 0;

            return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
        }

        MyQuaternion orderQuat = new MyQuaternion(q1.w, q1.z, q1.x, q1.y);

        anglesVector.y = (float)Math.Atan2(2f * orderQuat.x * orderQuat.w + 2f * orderQuat.y * orderQuat.z, 1 - 2f * (orderQuat.z * orderQuat.z + orderQuat.y * orderQuat.y));
        anglesVector.x = (float)Math.Asin(2f * (orderQuat.x * orderQuat.z - orderQuat.w * orderQuat.y));
        anglesVector.z = (float)Math.Atan2(2f * orderQuat.x * orderQuat.y + 2f * orderQuat.z * orderQuat.w, 1 - 2f * (orderQuat.y * orderQuat.y + orderQuat.z * orderQuat.z));

        return NormalizeAngles(anglesVector * Mathf.Rad2Deg);
    }

    public static MyQuaternion FromEulerToQuat(float x, float y, float z)
    {
        //Cada coordenada de los Euler representa en ese eje que tan rotado esta el objeto 

        float xInRad = Mathf.Deg2Rad * x * 0.5f; //Lo paso a radianes para poder trabajar con seno y coseno
        float yInRad = Mathf.Deg2Rad * y * 0.5f;
        float zInRad = Mathf.Deg2Rad * z * 0.5f;

        //Teniendo en cuenta la fórmula para rotar en 3D Cos(a/2) + iSin(a/2) + jSin(a/2) + kSin(a/2) calcula la rotacion en cada uno de los ejes.
        MyQuaternion qx = new MyQuaternion(Mathf.Sin(xInRad), 0, 0, Mathf.Cos(xInRad));
        MyQuaternion qy = new MyQuaternion(0, Mathf.Sin(yInRad), 0, Mathf.Cos(yInRad));
        MyQuaternion qz = new MyQuaternion(0, 0, Mathf.Sin(zInRad), Mathf.Cos(zInRad));

        MyQuaternion result = qy * qx * qz; //Hago la multiplicación para aplicar la rotación. Es en ese orden por como maneja el orden unity.

        return new MyQuaternion(result.x, result.y, result.z, result.w);
    }

    public static MyQuaternion FromEulerToQuat(Vector3 euler)
    {
        return FromEulerToQuat(euler.x, euler.y, euler.z);
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
        float magnitude = Mathf.Sqrt(Dot(q, q));

        if (magnitude < kEpsilon)
        {
            return identity;
        }

        return new MyQuaternion(q.x / magnitude, q.y / magnitude, q.z / magnitude, q.w / magnitude);
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
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w); //Incorpora el Equals de float
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z}, {w})";
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

    public static bool operator ==(MyQuaternion lhs, MyQuaternion rhs) //Para quaternions normalizados un producto punto de 1 indica que los quaternions son idénticos,
                                                                       //y cualquier valor cercano a 1 indica que son casi idénticos, dentro de un margen de error
    {
        return IsEqualUsingDot(Dot(lhs, rhs));
    }

    public static bool operator !=(MyQuaternion lhs, MyQuaternion rhs)
    {
        return !(lhs == rhs);
    }

    #endregion
}
