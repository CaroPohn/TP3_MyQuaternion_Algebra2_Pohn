using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMatrix4x4 : IEquatable<MyMatrix4x4>, IFormattable
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

    //
    // Resumen:
    //     Returns a matrix with all elements set to zero (Read Only).
    public static MyMatrix4x4 zero
    {
        get
        {
            return new MyMatrix4x4(
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0),
                new Vector4(0, 0, 0, 0)
            );
        }
    }
    //
    // Resumen:
    //     Returns the identity matrix (Read Only).
    public static MyMatrix4x4 identity
    {
        get
        {
            return new MyMatrix4x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            );
        }
    }
    //
    // Resumen:
    //     Attempts to get a rotation quaternion from this matrix.
    public MyQuaternion rotation
    {
        get
        {
            // https://math.stackexchange.com/questions/237369/given-this-transformation-matrix-how-do-i-decompose-it-into-translation-rotati

            // First we get the rotation matrix again by dividing the scales that affect every axis

            Vector3 scales = lossyScale;

            float rm00 = m00 / scales.x;
            float rm10 = m10 / scales.x;
            float rm20 = m20 / scales.x;

            float rm01 = m01 / scales.y;
            float rm11 = m11 / scales.y;
            float rm21 = m21 / scales.y;

            float rm02 = m02 / scales.z;
            float rm12 = m12 / scales.z;
            float rm22 = m22 / scales.z;

            return MyQuaternion.GetQuaternionFromRotationMatrix(
                new Vector3(rm00, rm10, rm20),
                new Vector3(rm01, rm11, rm21),
                new Vector3(rm02, rm12, rm22)
            );
        }
    }
    //
    // Resumen:
    //     Attempts to get a scale value from the matrix. (Read Only)
    public Vector3 lossyScale =>
        // https://math.stackexchange.com/questions/237369/given-this-transformation-matrix-how-do-i-decompose-it-into-translation-rotati
        // Extracting scale is taking the first three column vectors and obtaining its magnitude (As that is what it gives after multiplying)
        new(
            new Vector3(m00, m10, m20).magnitude,
            new Vector3(m01, m11, m21).magnitude,
            new Vector3(m02, m12, m22).magnitude
        );

    //
    // Resumen:
    //     Checks whether this is an identity matrix. (Read Only)
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
    //
    // Resumen:
    //     The determinant of the matrix. (Read Only)
    public float determinant
    {
        get
        {
            return m03 * m12 * m21 * m30 - m02 * m13 * m21 * m30 -
                m03 * m11 * m22 * m30 + m01 * m13 * m22 * m30 +
                m02 * m11 * m23 * m30 - m01 * m12 * m23 * m30 -
                m03 * m12 * m20 * m31 + m02 * m13 * m20 * m31 +
                m03 * m10 * m22 * m31 - m00 * m13 * m22 * m31 -
                m02 * m10 * m23 * m31 + m00 * m12 * m23 * m31 +
                m03 * m11 * m20 * m32 - m01 * m13 * m20 * m32 -
                m03 * m10 * m21 * m32 + m00 * m13 * m21 * m32 +
                m01 * m10 * m23 * m32 - m00 * m11 * m23 * m32 -
                m02 * m11 * m20 * m33 + m01 * m12 * m20 * m33 +
                m02 * m10 * m21 * m33 - m00 * m12 * m21 * m33 -
                m01 * m10 * m22 * m33 + m00 * m11 * m22 * m33;
        }
    }
    //
    // Resumen:
    //     Returns the transpose of this matrix (Read Only).
    public MyMatrix4x4 transpose
    {
        get
        {
            return new MyMatrix4x4(
                new Vector4(m00, m10, m20, m30),
                new Vector4(m01, m11, m21, m31),
                new Vector4(m02, m12, m22, m32),
                new Vector4(m03, m13, m23, m33)
            );
        }
    }

    //
    // Resumen:
    //     The inverse of this matrix. (Read Only)
    public MyMatrix4x4 inverse
    {
        get
        {
            // The inverse of a matrix is such that if multiplied by the original it would result in the identity matrix.   
            // obtained using https://euclideanspace.com/maths/algebra/matrix/functions/inverse/index.htm

            float newM00 = m12 * m23 * m31 - m13 * m22 * m31 + m13 * m21 * m32 - m11 * m23 * m32 - m12 * m21 * m33 + m11 * m22 * m33;
            float newM01 = m03 * m22 * m31 - m02 * m23 * m31 - m03 * m21 * m32 + m01 * m23 * m32 + m02 * m21 * m33 - m01 * m22 * m33;
            float newM02 = m02 * m13 * m31 - m03 * m12 * m31 + m03 * m11 * m32 - m01 * m13 * m32 - m02 * m11 * m33 + m01 * m12 * m33;
            float newM03 = m03 * m12 * m21 - m02 * m13 * m21 - m03 * m11 * m22 + m01 * m13 * m22 + m02 * m11 * m23 - m01 * m12 * m23;
            float newM10 = m13 * m22 * m30 - m12 * m23 * m30 - m13 * m20 * m32 + m10 * m23 * m32 + m12 * m20 * m33 - m10 * m22 * m33;
            float newM11 = m02 * m23 * m30 - m03 * m22 * m30 + m03 * m20 * m32 - m00 * m23 * m32 - m02 * m20 * m33 + m00 * m22 * m33;
            float newM12 = m03 * m12 * m30 - m02 * m13 * m30 - m03 * m10 * m32 + m00 * m13 * m32 + m02 * m10 * m33 - m00 * m12 * m33;
            float newM13 = m02 * m13 * m20 - m03 * m12 * m20 + m03 * m10 * m22 - m00 * m13 * m22 - m02 * m10 * m23 + m00 * m12 * m23;
            float newM20 = m11 * m23 * m30 - m13 * m21 * m30 + m13 * m20 * m31 - m10 * m23 * m31 - m11 * m20 * m33 + m10 * m21 * m33;
            float newM21 = m03 * m21 * m30 - m01 * m23 * m30 - m03 * m20 * m31 + m00 * m23 * m31 + m01 * m20 * m33 - m00 * m21 * m33;
            float newM22 = m01 * m13 * m30 - m03 * m11 * m30 + m03 * m10 * m31 - m00 * m13 * m31 - m01 * m10 * m33 + m00 * m11 * m33;
            float newM23 = m03 * m11 * m20 - m01 * m13 * m20 - m03 * m10 * m21 + m00 * m13 * m21 + m01 * m10 * m23 - m00 * m11 * m23;
            float newM30 = m12 * m21 * m30 - m11 * m22 * m30 - m12 * m20 * m31 + m10 * m22 * m31 + m11 * m20 * m32 - m10 * m21 * m32;
            float newM31 = m01 * m22 * m30 - m02 * m21 * m30 + m02 * m20 * m31 - m00 * m22 * m31 - m01 * m20 * m32 + m00 * m21 * m32;
            float newM32 = m02 * m11 * m30 - m01 * m12 * m30 - m02 * m10 * m31 + m00 * m12 * m31 + m01 * m10 * m32 - m00 * m11 * m32;
            float newM33 = m01 * m12 * m20 - m02 * m11 * m20 + m02 * m10 * m21 - m00 * m12 * m21 - m01 * m10 * m22 + m00 * m11 * m22;

            return new MyMatrix4x4(
                new Vector4(newM00, newM01, newM02, newM03),
                new Vector4(newM10, newM11, newM12, newM13),
                new Vector4(newM20, newM21, newM22, newM23),
                new Vector4(newM30, newM31, newM32, newM33)
            );
        }
    }

    public static float Determinant(MyMatrix4x4 m)
    {
        return m.determinant;
    }

    public static MyMatrix4x4 Inverse(MyMatrix4x4 m)
    {
        return m.inverse;
    }

    //
    // Resumen:
    //     Create a "look at" matrix.
    //
    // Par?metros:
    //   from:
    //     The source point.
    //
    //   to:
    //     The target point.
    //
    //   up:
    //     The vector describing the up direction (typically Vector3.up).
    //
    // Devuelve:
    //     The resulting transformation matrix.
    public static MyMatrix4x4 LookAt(Vector3 from, Vector3 to, Vector3 up)
    {
        return MyMatrix4x4.TRS(from, MyQuaternion.LookRotation(to - from, up), new Vector3(1f, 1f, 1f));
    }
    //
    // Resumen:
    //     Creates a rotation matrix.
    //
    // Par?metros:
    //   q:
    public static MyMatrix4x4 Rotate(MyQuaternion q)
    {
        // https://euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
        // To get the rotation from the quaternion you use the equation q*p*conj(q) = Mp, where M is the matrix we want.

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
    //
    // Resumen:
    //     Creates a scaling matrix.
    //
    // Par?metros:
    //   vector:
    public static MyMatrix4x4 Scale(Vector3 vector)
    {
        // https://learnopengl.com/Getting-started/Transformations#:~:text=solving%20linear%20equations.-,Scaling,-When%20we%27re%20scaling

        return new MyMatrix4x4(
            new Vector4(vector.x, 0, 0, 0),
            new Vector4(0, vector.y, 0, 0),
            new Vector4(0, 0, vector.z, 0),
            new Vector4(0, 0, 0, 1)
        );
    }
    //
    // Resumen:
    //     Creates a translation matrix.
    //
    // Par?metros:
    //   vector:
    public static MyMatrix4x4 Translate(Vector3 vector)
    {
        return new MyMatrix4x4(
            new Vector4(1, 0, 0, vector.x),
            new Vector4(0, 1, 0, vector.y),
            new Vector4(0, 0, 1, vector.z),
            new Vector4(0, 0, 0, 1)
        );
    }

    public static MyMatrix4x4 Transpose(MyMatrix4x4 m)
    {
        return m.transpose;
    }
    //
    // Resumen:
    //     Creates a translation, rotation and scaling matrix.
    //
    // Par?metros:
    //   pos:
    //
    //   q:
    //
    //   s:
    public static MyMatrix4x4 TRS(Vector3 pos, MyQuaternion q, Vector3 s)
    {
        return Translate(pos) * Rotate(q) * Scale(s);
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
    //
    // Resumen:
    //     Get a column of the matrix.
    //
    // Par?metros:
    //   index:
    public Vector4 GetColumn(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector4(m00, m10, m20, m30);
            case 1:
                return new Vector4(m01, m11, m21, m31);
            case 2:
                return new Vector4(m02, m12, m22, m32);
            case 3:
                return new Vector4(m03, m13, m23, m33);
            default:
                throw new IndexOutOfRangeException("Not in range!");
        }
    }

    //
    // Resumen:
    //     Get position vector from the matrix.
    public Vector3 GetPosition()
    {
        return new Vector3(m03, m13, m23);
    }
    //
    // Resumen:
    //     Returns a row of the matrix.
    //
    // Par?metros:
    //   index:
    public Vector4 GetRow(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector4(m00, m01, m02, m03);
            case 1:
                return new Vector4(m10, m11, m12, m13);
            case 2:
                return new Vector4(m20, m21, m22, m23);
            case 3:
                return new Vector4(m30, m31, m32, m33);
            default:
                throw new IndexOutOfRangeException("Not in range!");
        }
    }
    //
    // Resumen:
    //     Transforms a position by this matrix (generic).
    //
    // Par?metros:
    //   point:
    public Vector3 MultiplyPoint(Vector3 point)
    {
        Vector3 vector3 = MultiplyPoint3x4(point);

        // TODO why is this being calculated? third row should be all zero
        float num = 1f / ((float)((double)this.m30 * (double)point.x + (double)this.m31 * (double)point.y + (double)this.m32 * (double)point.z) + this.m33);
        vector3.x *= num;
        vector3.y *= num;
        vector3.z *= num;

        return vector3;
    }
    //
    // Resumen:
    //     Transforms a position by this matrix (fast).
    //
    // Par?metros:
    //   point:
    public Vector3 MultiplyPoint3x4(Vector3 point)
    {
        return new Vector3(
            (this.m00 * point.x + this.m01 * point.y + this.m02 * point.z) + this.m03,
            (this.m10 * point.x + this.m11 * point.y + this.m12 * point.z) + this.m13,
            (this.m20 * point.x + this.m21 * point.y + this.m22 * point.z) + this.m23
        );
    }
    //
    // Resumen:
    //     Transforms a direction by this matrix.
    //
    // Par?metros:
    //   vector:
    public Vector3 MultiplyVector(Vector3 vector)
    {
        // this doesnt change position as it is a direction

        return new Vector3(
            (this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z),
            (this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z),
            (this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z)
        );
    }
    //
    // Resumen:
    //     Sets a column of the matrix.
    //
    // Par?metros:
    //   index:
    //
    //   column:
    public void SetColumn(int index, Vector4 column)
    {
        switch (index)
        {
            case 0:
                m00 = column.x;
                m10 = column.y;
                m20 = column.z;
                m30 = column.w;
                break;
            case 1:
                m01 = column.x;
                m11 = column.y;
                m21 = column.z;
                m31 = column.w;
                break;
            case 2:
                m02 = column.x;
                m12 = column.y;
                m22 = column.z;
                m32 = column.w;
                break;
            case 3:
                m03 = column.x;
                m13 = column.y;
                m23 = column.z;
                m33 = column.w;
                break;
            default:
                throw new IndexOutOfRangeException("Not in range!");
        }
    }
    //
    // Resumen:
    //     Sets a row of the matrix.
    //
    // Par?metros:
    //   index:
    //
    //   row:
    public void SetRow(int index, Vector4 row)
    {
        switch (index)
        {
            case 0:
                m00 = row.x;
                m01 = row.y;
                m02 = row.z;
                m03 = row.w;
                break;
            case 1:
                m10 = row.x;
                m11 = row.y;
                m12 = row.z;
                m13 = row.w;
                break;
            case 2:
                m20 = row.x;
                m21 = row.y;
                m22 = row.z;
                m23 = row.w;
                break;
            case 3:
                m30 = row.x;
                m31 = row.y;
                m32 = row.z;
                m33 = row.w;
                break;
            default:
                throw new IndexOutOfRangeException("Not in range!");
        }
    }
    //
    // Resumen:
    //     Sets this matrix to a translation, rotation and scaling matrix.
    //
    // Par?metros:
    //   pos:
    //
    //   q:
    //
    //   s:
    public void SetTRS(Vector3 pos, MyQuaternion q, Vector3 s)
    {
        MyMatrix4x4 trsMatrix = TRS(pos, q, s);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                this[i, j] = trsMatrix[i, j];
            }
        }
    }

    //
    // Resumen:
    //     Returns a formatted string for this matrix.
    //
    // Par?metros:
    //   format:
    //     A numeric format string.
    //
    //   formatProvider:
    //     An object that specifies culture-specific formatting.
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return $"{m00} {m01} {m02} {m03}\n {m10} {m11} {m12} {m13}\n {m20} {m21} {m22} {m23}\n {m30} {m31} {m32} {m33}";
    }

    public bool ValidTRS()
    {
        // A TRS is valid if the 3x3 top left matrix is an orthogonal matrix, 
        // That is it is a square matrix and it's columns are orthonormal vectors.

        // To check this we can use the dot product between the columns.

        return Vector3.Dot(new Vector3(m00, m10, m20), new Vector3(m01, m11, m21)) <= float.Epsilon &&
               Vector3.Dot(new Vector3(m01, m11, m21), new Vector3(m02, m12, m22)) <= float.Epsilon &&
               Vector3.Dot(new Vector3(m00, m10, m20), new Vector3(m02, m12, m22)) <= float.Epsilon;
    }

    public static MyMatrix4x4 operator *(MyMatrix4x4 lhs, MyMatrix4x4 rhs)
    {
        return new MyMatrix4x4(
            new Vector4(
                lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
                lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31,
                lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32,
                lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33
                ),
            new Vector4(
                lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30,
                lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31,
                lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,
                lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33
                ),
            new Vector4(
                lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30,
                lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31,
                lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,
                lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33
                ),
            new Vector4(
                lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30,
                lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31,
                lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,
                lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33
            )
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