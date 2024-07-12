using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using CustomMath;

public class MyMatrix4x4 
{
    public float m00;
    public float m33;
    public float m23;
    public float m13;
    public float m03;
    public float m32;
    public float m22;
    public float m02;
    public float m12;
    public float m21;
    public float m11;
    public float m01;
    public float m30;
    public float m20;
    public float m10;
    public float m31;

    public MyMatrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
    {
        m00 = column0[0];
        m10 = column0[1];
        m20 = column0[2];
        m30 = column0[3];

        m01 = column1[0];
        m11 = column1[1];
        m21 = column1[2];
        m31 = column1[3];

        m02 = column2[0];
        m12 = column2[1];
        m22 = column2[2];
        m32 = column2[3];

        m03 = column3[0];
        m13 = column3[1];
        m23 = column3[2];
        m33 = column3[3];
    }

    public MyMatrix4x4()
    {
    }

    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return m00;
                case 1: return m10;
                case 2: return m20;
                case 3: return m30;
                case 4: return m01;
                case 5: return m11;
                case 6: return m21;
                case 7: return m31;
                case 8: return m02;
                case 9: return m12;
                case 10: return m22;
                case 11: return m32;
                case 12: return m03;
                case 13: return m13;
                case 14: return m23;
                case 15: return m33;
                default:
                    throw new IndexOutOfRangeException("Invalid matrix index!");
            }
        }

        set
        {
            switch (index)
            {
                case 0: m00 = value; break;
                case 1: m10 = value; break;
                case 2: m20 = value; break;
                case 3: m30 = value; break;
                case 4: m01 = value; break;
                case 5: m11 = value; break;
                case 6: m21 = value; break;
                case 7: m31 = value; break;
                case 8: m02 = value; break;
                case 9: m12 = value; break;
                case 10: m22 = value; break;
                case 11: m32 = value; break;
                case 12: m03 = value; break;
                case 13: m13 = value; break;
                case 14: m23 = value; break;
                case 15: m33 = value; break;

                default:
                    throw new IndexOutOfRangeException("Invalid matrix index!");
            }
        }
    }

    public float this[int row, int column]
    {
        get
        {
            return this[row + column * 4];
        }

        set
        {
            this[row + column * 4] = value;
        }
    }

    public static MyMatrix4x4 zero =>
        new(
            new Vector4(0, 0, 0, 0),
            new Vector4(0, 0, 0, 0),
            new Vector4(0, 0, 0, 0),
            new Vector4(0, 0, 0, 0));

    public static MyMatrix4x4 Identity { get; } = new MyMatrix4x4
   (
       new Vector4(1, 0, 0, 0),
       new Vector4(0, 1, 0, 0),
       new Vector4(0, 0, 1, 0),
       new Vector4(0, 0, 0, 1)
   );

    //Obtiene la rotacion en una matriz y lo devuelve como un quaternion de rotacion
    public MyQuaternion rotation
    {
        get
        {
            //https://d3cw3dd2w32x2b.cloudfront.net/wp-content/uploads/2015/01/matrix-to-quat.pdf

            MyMatrix4x4 m = this;

            float trace = m.m00 + m.m11 + m.m22;
            float s;

            MyQuaternion q = new MyQuaternion();

            if (trace > 0)
            {
                s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                q.w = 0.25f / s;
                q.x = (m.m21 - m.m12) * s;
                q.y = (m.m02 - m.m20) * s;
                q.z = (m.m10 - m.m01) * s;
            }
            else
            {
                if (m.m00 > m.m11 && m.m00 > m.m22)
                {
                    s = 2.0f * Mathf.Sqrt(1.0f + m.m00 - m.m11 - m.m22);
                    q.w = (m.m21 - m.m12) / s;
                    q.x = 0.25f * s;
                    q.y = (m.m01 + m.m10) / s;
                    q.z = (m.m02 + m.m20) / s;
                }
                else if (m.m11 > m.m22)
                {
                    s = 2.0f * Mathf.Sqrt(1.0f + m.m11 - m.m00 - m.m22);
                    q.w = (m.m02 - m.m20) / s;
                    q.x = (m.m01 + m.m10) / s;
                    q.y = 0.25f * s;
                    q.z = (m.m12 + m.m21) / s;
                }
                else
                {
                    s = 2.0f * Mathf.Sqrt(1.0f + m.m22 - m.m00 - m.m11);
                    q.w = (m.m10 - m.m01) / s;
                    q.x = (m.m02 + m.m20) / s;
                    q.y = (m.m12 + m.m21) / s;
                    q.z = 0.25f * s;
                }
            }

            // Normalizar el cuaternión
            float length = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            q.x /= length;
            q.y /= length;
            q.z /= length;
            q.w /= length;

            return q;
        }
    }

    //Devuelve la escala real del objeto. Esto es en caso de que se apliquen rotaciones y otros cálculos, donde se pierde la escala
    public Vec3 lossyScale => new(GetColumn(0).magnitude, GetColumn(1).magnitude, GetColumn(2).magnitude);

    //Chequea si es matriz identidad
    public bool IsIdentity
    {
        get
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == j && this[i, j] != 1)
                    {
                        return false;
                    }
                    else if (this[i, j] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    //El determinante de esta matriz
    public float determinant
    {
        get
        {
            return Determinant(this);
        }
    }


    //La transpuesta de esta matriz
    public MyMatrix4x4 transpose
    {
        get
        {
            return Transpose(this);
        }
    }

    //La inversa de esta matriz
    public MyMatrix4x4 inverse
    {
        get
        {
            return Inverse(this);
        }
    }

    public static float Determinant(MyMatrix4x4 m)
    {
        float a = m.m00;
        float b = m.m01;
        float c = m.m02;
        float d = m.m03;

        //m00 m01 m02 m03
        //m10 m11 m12 m13
        //m20 m21 m22 m23
        //m30 m31 m32 m33

        // aDeterminant 
        // m11 m12 m13
        // m21 m22 m23
        // m31 m32 m33
        float aDeterminant = m.m11 * (m.m22 * m.m33 - m.m23 * m.m32) - m.m12 * (m.m21 * m.m33 - m.m23 * m.m31) + m.m13 * (m.m21 * m.m32 - m.m22 * m.m31);

        // bDeterminant 
        // m10 m12 m13
        // m20 m22 m23
        // m30 m32 m33
        float bDeterminant = m.m10 * (m.m22 * m.m33 - m.m23 * m.m32) - m.m12 * (m.m20 * m.m33 - m.m23 * m.m30) + m.m13 * (m.m20 * m.m32 - m.m22 * m.m30);

        // cDeterminant 
        // m10 m11 m13
        // m20 m21 m23
        // m30 m31 m33
        float cDeterminant = m.m10 * (m.m21 * m.m33 - m.m23 * m.m31) - m.m11 * (m.m20 * m.m33 - m.m23 * m.m30) + m.m13 * (m.m20 * m.m31 - m.m21 * m.m30);

        // dDeterminant 
        // m10 m11 m12
        // m20 m21 m22
        // m30 m31 m32
        float dDeterminant = m.m10 * (m.m21 * m.m32 - m.m22 * m.m31) - m.m11 * (m.m20 * m.m32 - m.m22 * m.m30) + m.m12 * (m.m20 * m.m31 - m.m21 * m.m30);

        return a * aDeterminant - b * bDeterminant + c * cDeterminant - d * dDeterminant;
    }

    //= la conjugada de la matriz / su determinante, es decir, la conjugada escalada por la inversa de la determinante
    //Una matriz por su inversa devuelve la matriz identidad
    public static MyMatrix4x4 Inverse(MyMatrix4x4 m) //Devuelve la inversa de la matriz ingresada
    {
        float detA = Determinant(m); //Debe tener determinante, de otra forma, no es inversible

        if (detA == 0)
            return zero;

        MyMatrix4x4 aux = new MyMatrix4x4() //Conjugada
        {
            // sacar el determinante de cada una de esas posiciones
            //------0--------- 
            m00 = m.m11 * m.m22 * m.m33 + m.m12 * m.m23 * m.m31 + m.m13 * m.m21 * m.m32 - m.m11 * m.m23 * m.m32 - m.m12 * m.m21 * m.m33 - m.m13 * m.m22 * m.m31,
            m01 = m.m01 * m.m23 * m.m32 + m.m02 * m.m21 * m.m33 + m.m03 * m.m22 * m.m31 - m.m01 * m.m22 * m.m33 - m.m02 * m.m23 * m.m31 - m.m03 * m.m21 * m.m32,
            m02 = m.m01 * m.m12 * m.m33 + m.m02 * m.m13 * m.m32 + m.m03 * m.m11 * m.m32 - m.m01 * m.m13 * m.m32 - m.m02 * m.m11 * m.m33 - m.m03 * m.m12 * m.m31,
            m03 = m.m01 * m.m13 * m.m22 + m.m02 * m.m11 * m.m23 + m.m03 * m.m12 * m.m21 - m.m01 * m.m12 * m.m23 - m.m02 * m.m13 * m.m21 - m.m03 * m.m11 * m.m22,
            //-------1--------         
            m10 = m.m10 * m.m23 * m.m32 + m.m12 * m.m20 * m.m33 + m.m13 * m.m22 * m.m30 - m.m10 * m.m22 * m.m33 - m.m12 * m.m23 * m.m30 - m.m13 * m.m20 * m.m32,
            m11 = m.m00 * m.m22 * m.m33 + m.m02 * m.m23 * m.m30 + m.m03 * m.m20 * m.m32 - m.m00 * m.m23 * m.m32 - m.m02 * m.m20 * m.m33 - m.m03 * m.m22 * m.m30,
            m12 = m.m00 * m.m13 * m.m32 + m.m02 * m.m10 * m.m33 + m.m03 * m.m12 * m.m30 - m.m00 * m.m12 * m.m33 - m.m02 * m.m13 * m.m30 - m.m03 * m.m10 * m.m32,
            m13 = m.m00 * m.m12 * m.m23 + m.m02 * m.m13 * m.m20 + m.m03 * m.m10 * m.m22 - m.m00 * m.m13 * m.m22 - m.m02 * m.m10 * m.m23 - m.m03 * m.m12 * m.m20,
            //-------2--------         
            m20 = m.m10 * m.m21 * m.m33 + m.m11 * m.m23 * m.m30 + m.m13 * m.m20 * m.m31 - m.m10 * m.m23 * m.m31 - m.m11 * m.m20 * m.m33 - m.m13 * m.m31 * m.m30,
            m21 = m.m00 * m.m23 * m.m31 + m.m01 * m.m20 * m.m33 + m.m03 * m.m21 * m.m30 - m.m00 * m.m21 * m.m33 - m.m01 * m.m23 * m.m30 - m.m03 * m.m20 * m.m31,
            m22 = m.m00 * m.m11 * m.m33 + m.m01 * m.m13 * m.m31 + m.m03 * m.m10 * m.m31 - m.m00 * m.m13 * m.m31 - m.m01 * m.m10 * m.m33 - m.m03 * m.m11 * m.m30,
            m23 = m.m00 * m.m13 * m.m21 + m.m01 * m.m10 * m.m23 + m.m03 * m.m11 * m.m31 - m.m00 * m.m11 * m.m23 - m.m01 * m.m13 * m.m20 - m.m03 * m.m10 * m.m21,
            //------3---------         
            m30 = m.m10 * m.m22 * m.m31 + m.m11 * m.m20 * m.m32 + m.m12 * m.m21 * m.m30 - m.m00 * m.m21 * m.m32 - m.m11 * m.m22 * m.m30 - m.m12 * m.m20 * m.m31,
            m31 = m.m00 * m.m21 * m.m32 + m.m01 * m.m22 * m.m30 + m.m02 * m.m20 * m.m31 - m.m00 * m.m22 * m.m31 - m.m01 * m.m20 * m.m32 - m.m02 * m.m21 * m.m30,
            m32 = m.m00 * m.m12 * m.m31 + m.m01 * m.m10 * m.m32 + m.m02 * m.m11 * m.m30 - m.m00 * m.m11 * m.m32 - m.m01 * m.m12 * m.m30 - m.m02 * m.m10 * m.m31,
            m33 = m.m00 * m.m11 * m.m22 + m.m01 * m.m12 * m.m20 + m.m02 * m.m10 * m.m21 - m.m00 * m.m12 * m.m21 - m.m01 * m.m10 * m.m22 - m.m02 * m.m11 * m.m20
        };

        MyMatrix4x4 ret = new MyMatrix4x4()
        {
            m00 = aux.m00 / detA,
            m01 = aux.m01 / detA,
            m02 = aux.m02 / detA,
            m03 = aux.m03 / detA,
            m10 = aux.m10 / detA,
            m11 = aux.m11 / detA,
            m12 = aux.m12 / detA,
            m13 = aux.m13 / detA,
            m20 = aux.m20 / detA,
            m21 = aux.m21 / detA,
            m22 = aux.m22 / detA,
            m23 = aux.m23 / detA,
            m30 = aux.m30 / detA,
            m31 = aux.m31 / detA,
            m32 = aux.m32 / detA,
            m33 = aux.m33 / detA

        };
        return ret;
    }

    public static MyMatrix4x4 Transpose(MyMatrix4x4 m)//Espeja respecto de la diagonal principal
    {
        return new MyMatrix4x4()
        {
            m01 = m.m10,
            m02 = m.m20,
            m03 = m.m30,
            m10 = m.m01,
            m12 = m.m21,
            m13 = m.m31,
            m20 = m.m02,
            m21 = m.m12,
            m23 = m.m32,
            m30 = m.m03,
            m31 = m.m13,
            m32 = m.m23,
        };
    }

    //A partir de un quaternion arma la matriz de rotacion
    //La matriz de rotacion es una matriz que aplica 3 rotaciones, una para cada eje (ya que se arma multiplicando la matriz de rotacion en x, y, z)
    public static MyMatrix4x4 Rotate(MyQuaternion q)
    {
        //Cuando multiplico cualquier vector por esta matriz me devuelve el vector rotado.

        return new MyMatrix4x4(
            new Vector4(
                1f - 2f * (q.y * q.y + q.z * q.z),
                2f * q.x * q.y - 2f * q.z * q.w,
                2f * q.x * q.z + 2f * q.y * q.w,
                0f
                ),
            new Vector4(
                2f * q.x * q.y + 2f * q.z * q.w,
                1f - 2f * q.x * q.x - 2f * q.z * q.z,
                2f * q.y * q.z - 2f * q.x * q.w,
                0f
                ),
            new Vector4(
                2f * q.x * q.z - 2f * q.y * q.w,
                2 * q.y * q.z + 2 * q.x * q.w,
                1 - 2 * q.x * q.x - 2 * q.y * q.y,
                0
                ),
            new Vector4(0, 0, 0, 1)
        );
    }

    //Crea una matriz de escala que escala cualquier vector que se multiplique por ella
    public static MyMatrix4x4 Scale(Vec3 vector)
    {
        return new MyMatrix4x4(
            new Vector4(vector.x, 0, 0, 0),
            new Vector4(0, vector.y, 0, 0),
            new Vector4(0, 0, vector.z, 0),
            new Vector4(0, 0, 0, 1)
        );
    }

    //Crea una matriz de traslacion que, teniendo en cuenta una escala 1, representa la posicion con respecto al origen //En directX esta abajo
    public static MyMatrix4x4 Translate(Vec3 vector)
    {
        return new MyMatrix4x4(
            new Vector4(1, 0, 0, vector.x),
            new Vector4(0, 1, 0, vector.y),
            new Vector4(0, 0, 1, vector.z),
            new Vector4(0, 0, 0, 1)
        );
    }

    public static MyMatrix4x4 TRS(Vec3 pos, MyQuaternion q, Vec3 s) //Devuelve la matriz TRS de los valores ingresados
    {
        return (Translate(pos) * Rotate(q) * Scale(s)); //Va en este orden porque no es conmutativo. Primero multiplica scale y rotate y luego lo traslada.
    }

    public bool Equals(MyMatrix4x4 other)
    {
        return other != null && Mathf.Approximately(m00, other.m00) &&
               Mathf.Approximately(m01, other.m01) &&
               Mathf.Approximately(m02, other.m02) &&
               Mathf.Approximately(m03, other.m03) &&
               Mathf.Approximately(m10, other.m10) &&
               Mathf.Approximately(m11, other.m11) &&
               Mathf.Approximately(m12, other.m12) &&
               Mathf.Approximately(m13, other.m13) &&
               Mathf.Approximately(m20, other.m20) &&
               Mathf.Approximately(m21, other.m21) &&
               Mathf.Approximately(m22, other.m22) &&
               Mathf.Approximately(m23, other.m23) &&
               Mathf.Approximately(m30, other.m30) &&
               Mathf.Approximately(m31, other.m31) &&
               Mathf.Approximately(m32, other.m32) &&
               Mathf.Approximately(m33, other.m33);
    }

    public Vector4 GetColumn(int i) //Devuelve la columna
    {
        return new Vector4(this[0, i], this[1, i], this[2, i], this[3, i]);
    }

    //Obtiene el vector posicion en la matriz
    public Vec3 GetPosition()
    {
        return new Vec3(m03, m13, m23);
    }

    public Vector4 GetRow(int index) //Devuelve la fila
    {
        return index switch
        {
            0 => new Vector4(m00, m01, m02, m03),
            1 => new Vector4(m10, m11, m12, m13),
            2 => new Vector4(m20, m21, m22, m23),
            3 => new Vector4(m30, m31, m32, m33),
            _ => throw new IndexOutOfRangeException("Index out of Range!")
        };
    }

    public Vec3 MultiplyPoint(Vec3 p)
    {
        Vec3 v3;

        v3.x = (float)((double)m00 * (double)p.x + (double)m01 * (double)p.y + (double)m02 * (double)p.z) + m03;
        v3.y = (float)((double)m10 * (double)p.x + (double)m11 * (double)p.y + (double)m12 * (double)p.z) + m13;
        v3.z = (float)((double)m20 * (double)p.x + (double)m21 * (double)p.y + (double)m22 * (double)p.z) + m23;

        //Creo un valor scalar
        float scalar = 1f / ((float)((double)m30 * (double)p.x + (double)m31 * (double)p.y + (double)m32 * (double)p.z) + m33);
        v3.x *= scalar;
        v3.y *= scalar;
        v3.z *= scalar;

        return v3;
    }

    public Vec3 MultiplyPoint3x4(Vec3 p) //Multiplica las componentes del vector en la matriz (X, Y y Z pero no ignoro W)
    {
        Vec3 v3;
        v3.x = (float)((double)m00 * (double)p.x + (double)m01 * (double)p.y + (double)m02 * (double)p.z) + m03;
        v3.y = (float)((double)m10 * (double)p.x + (double)m11 * (double)p.y + (double)m12 * (double)p.z) + m13;
        v3.z = (float)((double)m20 * (double)p.x + (double)m21 * (double)p.y + (double)m22 * (double)p.z) + m23;
        return v3;
    }

    public Vec3 MultiplyVector(Vec3 v) //Multiplica las componentes del vector en la matriz (pero solo en X, Y y Z; ignorando W)
    {
        Vec3 v3; //No se tienen en cuenta ni la 4ta fila ni la 4ta columna
        v3.x = (float)((double)m00 * (double)v.x + (double)m01 * (double)v.y + (double)m02 * (double)v.z);
        v3.y = (float)((double)m10 * (double)v.x + (double)m11 * (double)v.y + (double)m12 * (double)v.z);
        v3.z = (float)((double)m20 * (double)v.x + (double)m21 * (double)v.y + (double)m22 * (double)v.z);

        return v3;
    }

    public void SetColumn(int index, Vector4 col) //Setea la columna
    {
        this[0, index] = col.x;
        this[1, index] = col.y;
        this[2, index] = col.z;
        this[3, index] = col.w;
    }

    public void SetRow(int index, Vector4 row) //Setea la fila
    {
        this[index, 0] = row.x;
        this[index, 1] = row.y;
        this[index, 2] = row.z;
        this[index, 3] = row.w;
    }

    public void SetTRS(Vec3 pos, MyQuaternion q, Vec3 s)
    {
        MyMatrix4x4 trs = TRS(pos, q, s);

        for (int i = 0; i < 4; i++)
        {
            SetColumn(i, trs.GetColumn(i));
        }
    }

    public bool ValidTRS()
    {
        //Checks if every axis is orthogonal (aka everyone of them are perpendicular between them)

        float kEpsilon = 1E-25F;

        Vec3 column0 = new Vec3(m00, m10, m20);
        Vec3 column1 = new Vec3(m01, m11, m21);
        Vec3 column2 = new Vec3(m02, m12, m22);

        return Vec3.Dot(column0, column1) <= kEpsilon &&
               Vec3.Dot(column0, column2) <= kEpsilon &&
               Vec3.Dot(column1, column2) <= kEpsilon;
    }

    public static MyMatrix4x4 operator *(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        return new MyMatrix4x4(
            lhs * rhs.GetColumn(0),
            lhs * rhs.GetColumn(1),
            lhs * rhs.GetColumn(2),
            lhs * rhs.GetColumn(3)
            );
    }

    public static bool operator ==(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        return lhs != null && lhs.Equals(rhs);
    }
    public static bool operator !=(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        return !(lhs == rhs);
    }

    //Devuelve el vector4 modificado por cada una de las filas de la matriz.
    public static Vector4 operator *(MyMatrix4x4 lhs, Vector4 vector)
    {
        return new Vector4(
            lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w,
            lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w,
            lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w,
            lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w
            );
    }
}