using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class MyQuaternion
{
    #region PROPIEDADES

    public float x;
    public float y;
    public float z; //x, y, z son las partes imaginarias
    public float w; //parte real

    private static readonly MyQuaternion identityQuaternion = new MyQuaternion(0f, 0f, 0f, 1f); //Quaternion de identidad, no rotacion. Rotacion alineada con los ejes

    public const float kEpsilon = 1E-06f; // =0.000001, evita comparaciones de floats

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

    public MyQuaternion()
    {

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

    public Vec3 eulerAngles
    {
        get //Devuelve la rotacion de un quaternion en eulers
        {
           return QuatToEulerRad(this);
        }

        set //Crea un quaternion en base a la rotacion en eulers
        {
            MyQuaternion result = FromEulerToQuat(value);

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
            if (magnitude < kEpsilon)
            {
                return identity;
            }

            return new MyQuaternion(x / magnitude, y / magnitude, z / magnitude, w / magnitude);
        }
    }

    #endregion

    #region FUNCIONES
    public static MyQuaternion FromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        //Con producto cruz obtengo un vector perpendicular a los que tengo (axis)
        Vec3 axis = Vec3.Cross(fromDirection, toDirection);

        //Calculamos el angulo entre los dos vectores
        float angle = Vec3.Angle(fromDirection, toDirection);

        //Va a girar en el eje pasado la cantidad de ángulos pasados
        return AngleAxis(angle, axis);
    }

    public static MyQuaternion Inverse(MyQuaternion rotation)
    {
        return new MyQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
    }

    //Interpola esfericamente entre un quat a y un quat b en un factor t. T esta limitado entre 0 y 1.
    public static MyQuaternion Slerp(MyQuaternion a, MyQuaternion b, float t)
    {
        return SlerpUnclamped(a, b, Mathf.Clamp01(t));
    }

    //Interpola esfericamente entre un quat a y un quat b en un factor t. T no esta limitado entre 0 y 1.
    public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        //https://en.wikipedia.org/wiki/Slerp#:~:text=0%20and%C2%A01.-,Geometric%20slerp,-%5Bedit%5D formula

        MyQuaternion normA = a.normalized;
        MyQuaternion normB = b.normalized;

        float cosOmega = Dot(normA, normB);

        if (cosOmega < 0.0f) //Busca el camino mas corto //ortodromica
        {
            //Cambia el signo de la interpolacion para ir hacia el otro lado
            cosOmega = -cosOmega;
        }

        float coeff1, coeff2;

        float omega = Mathf.Acos(cosOmega);

        //Coeficientes de incidencia, mantiene el quaternion unitario
        coeff1 = Mathf.Sin((1 - t) * omega) / Mathf.Sin(omega);
        coeff2 = (cosOmega < 0.0f ? -1 : 1) * (Mathf.Sin(t * omega) / Mathf.Sin(omega));

        //Genera un nuevo vector multiplicando los componentes de ambos quat segun su coeficiente de incidencia
        return new MyQuaternion(
                coeff1 * normA.x + coeff2 * normB.x,
                coeff1 * normA.y + coeff2 * normB.y,
                coeff1 * normA.z + coeff2 * normB.z,
                coeff1 * normA.w + coeff2 * normB.w
            );
    }

    //Interpola linearmente entre un quat a y un quat b en un factor t y normaliza el resultado. T esta limitado entre 0 y 1.
    public static MyQuaternion Lerp(MyQuaternion a, MyQuaternion b, float t)
    {
        return LerpUnclamped(a, b, Mathf.Clamp01(t));
    }

    //Interpola linearmente entre un quat a y un quat b en un factor t y normaliza el resultado. T no esta limitado entre 0 y 1.
    public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
    {
        MyQuaternion result = identity;

        float timeLeft = 1 - t; //Primero se averigua el tiempo restante (para que la rotación llegue de “a” a “b”).

        if (Dot(a, b) >= 0) //Averigua el camino mas corto, dependiendo de eso se hace una suma o una resta para la fórmula de interpolación lineal 
        {
            result.x = (timeLeft * a.x) + (t * b.x);
            result.y = (timeLeft * a.y) + (t * b.y);
            result.z = (timeLeft * a.z) + (t * b.z);
            result.w = (timeLeft * a.w) + (t * b.w);
        }
        else
        {
            result.x = (timeLeft * a.x) - (t * b.x);
            result.y = (timeLeft * a.y) - (t * b.y);
            result.z = (timeLeft * a.z) - (t * b.z);
            result.w = (timeLeft * a.w) - (t * b.w);
        }

        result.Normalize();

        return result;
    }

    //Me devuelve un quaternion de rotacion que me va a modificar la rotacion en un angulo determinado alrededor de un eje especifico
    public static MyQuaternion AngleAxis(float angle, Vec3 axis)
    {
        axis.Normalize();

        axis *= Mathf.Sin(angle * Mathf.Deg2Rad * 0.5f); //Obtengo el eje rotado como se ve en la formula para eulers

        return new MyQuaternion(axis.x, axis.y, axis.z, Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f)); //Le paso los ejes rotados correspondientes y la parte real es el cos del angulo/2
    }

    //Representa una rotacion basada en una dirección foward y un up
    public static MyQuaternion LookRotation(Vec3 forward, Vec3 upwards)
    {
        //Setea los ejes que compondran la rotación del quaternion
        Vec3 forwardToUse = forward.normalized; 
        Vec3 rightToUse = Vec3.Cross(upwards, forward).normalized; //Obtenemos el eje faltante con producto cruz
        Vec3 upToUse = upwards.normalized; //Se normaliza para evitar ejes defasados 

        //Se crea la matriz de rotacion usando los valores de los ejes que obtuvimos
        //Cada fila es uno de los axis en orden x, y, z
        float m00 = rightToUse.x;
        float m01 = rightToUse.y;
        float m02 = rightToUse.z;

        float m10 = upToUse.x;
        float m11 = upToUse.y;
        float m12 = upToUse.z;

        float m20 = forwardToUse.x;
        float m21 = forwardToUse.y;
        float m22 = forwardToUse.z;

        //Formamos un quaternion en base a la fórmula de una matriz creada a partir de un cuaternion

        MyQuaternion result;
        float factor;

        //Se determina qué componente del cuaternión 4x(x, y, z o w) es más significativo basándose en los elementos de la matriz para evitar que en determinadas
        //situaciones puede volverse todo 0.

        if (m22 < 0) // sqr(X) + sqr(Y) > 1/2 que es lo mismo que |(X, Y)| > |(Z, W)| si estan normalizadas
        {
            //Comprueba si el componente x es mayor que el componente y se asegura de que el componente x no sea cero
            if (m00 > m11) //X > Y ?
            {
                //Se calcula el factor correspondiente para x 
                factor = 1 + m00 - m11 - m22; // sqr(X)

                //Se construye el cuaternión con las ecuaciones correctas.
                result = new MyQuaternion(factor, m10 + m01, m20 + m02, m12 - m21);
            }
            else
            {
                factor = 1 - m00 + m11 - m22;

                result = new MyQuaternion(m01 + m10, factor, m12 + m21, m20 - m02);
            }
        }
        else
        {
            if (m00 < -m11)
            {
                factor = 1 - m00 - m11 + m22;
                result = new MyQuaternion(m20 + m02, m12 + m21, factor, m01 - m10);
            }
            else
            {
                factor = 1 + m00 + m11 + m22; 
                result = new MyQuaternion(m12 - m21, m20 - m02, m01 - m10, factor);
            }
        }

        //Después de calcular el cuaternión con el componente dominante, se normaliza
        //Asegura que el cuaternión resultante tenga una magnitud de 1, haciendo que represente una rotación válida.
        result *= 0.5f / Mathf.Sqrt(factor);

        return result;
    }

    public static MyQuaternion LookRotation(Vec3 forward)
    {
        Vec3 upwards = Vec3.up; //Toma el del mundo

        forward.Normalize();

        Vec3 newRight = Vec3.Cross(upwards, forward).normalized; //Calculo el right de acuerdo al forward

        Vec3 newUp = Vec3.Cross(forward, newRight); //Calcula el up entre los dos ejes, para evitar posibles errores de escala

        return LookRotation(forward, newUp);
    }

    public static float Dot(MyQuaternion a, MyQuaternion b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public void SetLookRotation(Vec3 view, Vec3 up)
    {
        this.x = LookRotation(view, up).x;
        this.y = LookRotation(view, up).y;
        this.z = LookRotation(view, up).z;
        this.w = LookRotation(view, up).w;
    }

    public void SetLookRotation(Vec3 view)
    {
        Vec3 up = Vec3.up;
        SetLookRotation(view, up);
    }

    public static float Angle(MyQuaternion a, MyQuaternion b) //Devuelve el angulo en grados entre dos rotaciones. Va en un rango entre 0 y 180
    {
        float dot = Dot(a, b); //Desplazamiento entre las rotaciones
        float dotAbs = Math.Abs(dot);

        if (IsEqualUsingDot(Dot(a, b))) //el ángulo entre ambas rotaciones es 0 si ambos cuaterniones son iguales
        {
            return 0.0f;
        }
        else
        {
            return Mathf.Acos(Mathf.Min(dotAbs, 1f)) * 2.0f * Mathf.Rad2Deg; //180 max angulo (con el camino mas corto) y 180° = 1 Rad. Se usa el min para tomar el menor angulo
        }
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;

        while (angle < 0f)
            angle += 360;

        return angle;
    }

    private static Vec3 NormalizeAngles(Vec3 angles)
    {
        Vec3 normalizedAngles = new Vec3();

        normalizedAngles.x = NormalizeAngle(angles.x);
        normalizedAngles.y = NormalizeAngle(angles.y);
        normalizedAngles.z = NormalizeAngle(angles.z);

        return normalizedAngles;
    }

    public static Vec3 QuatToEulerRad(MyQuaternion q1)
    {
        float sqW = q1.w * q1.w;
        float sqX = q1.x * q1.x;
        float sqY = q1.y * q1.y;
        float sqZ = q1.z * q1.z;

        float unit = sqX + sqY + sqZ + sqW; //Chequea si el quat esta normalizado (da 1 si si, sino lo corrige)

        float test = q1.x * q1.w - q1.y * q1.z; //Obtiene el valor de x

        Vec3 anglesVector = new Vec3();

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

    public static MyQuaternion FromEulerToQuat(Vec3 euler)
    {
        return FromEulerToQuat(euler.x, euler.y, euler.z);
    }

    //Revierte AngleAxis, es decir, a partir de un quaternion nos devuelve el angulo y el axis
    public void ToAngleAxis(out float angle, out Vec3 axis)
    {
        MyQuaternion thisNormalized = this.normalized;

        // To obtain the angle we take it from the real part of the quaternion
        angle = 2.0f * Mathf.Acos(thisNormalized.w);

        // To obtain the axis values, first we check if we are almost an idenity quaternion
        float magnitude = Mathf.Sqrt(1f - thisNormalized.w * thisNormalized.w);

        // if magnitude is almost zero, we return any axis, as there is no rotation
        if (magnitude < 0.0001f)
        {
            axis = new Vec3(1, 0, 0);
        }
        else
        {
            // If we have a rotation, then we divide the imaginary values by the sin of the angle
            // Note: This does not perform well as we are diving by floats, but for understanding ill leave it like so
            axis = new Vec3(
                thisNormalized.x / Mathf.Sin(angle / 2f),
                thisNormalized.y / Mathf.Sin(angle / 2f),
                thisNormalized.z / Mathf.Sin(angle / 2f)
                );
        }
    }

    public static MyQuaternion RotateTowards(MyQuaternion from, MyQuaternion to, float maxDegreesDelta) //Mover del uno al otro, la cantidad de grados especificada.
    {
        float num = Angle(from, to);

        if (num == 0)
        {
            return to;
        }

        float t = Mathf.Min(1f, maxDegreesDelta / num);
        return SlerpUnclamped(from, to, t);
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

    public override string ToString()
    {
        return $"({x:F2}, {y:F2}, {z:F2}, {w:F2})";
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

    #endregion

    #region OPERADORES
    public static MyQuaternion operator *(MyQuaternion lhs, MyQuaternion rhs)
    {
        float x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
        float y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z;
        float z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x;
        float w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;

        return new MyQuaternion(x, y, z, w);
    }

    public static MyQuaternion operator *(MyQuaternion q, float value)
    {
        return new MyQuaternion(
            q.x * value,
            q.y * value,
            q.z * value,
            q.w * value
        );
    }

    public static Vec3 operator *(MyQuaternion rotation, Vec3 point)
    {
        MyQuaternion pureVectorQuaternion = new MyQuaternion(point.x, point.y, point.z, 0);
        MyQuaternion appliedPureQuaternion = rotation * pureVectorQuaternion * MyQuaternion.Inverse(rotation);

        return new Vec3(appliedPureQuaternion.x, appliedPureQuaternion.y, appliedPureQuaternion.z);
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
