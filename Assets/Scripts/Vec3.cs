using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CustomMath
{
    [System.Serializable]
    public struct Vec3 : IEquatable<Vec3>
    {
        #region Variables
        [SerializeField] public float x;
        [SerializeField] public float y;
        [SerializeField] public float z;

        public float sqrMagnitude { get { return (x * x + y * y + z * z); } }
        public Vec3 normalized { get { return new Vec3(x / magnitude, y / magnitude, z / magnitude); } }
        public float magnitude { get { return Mathf.Sqrt(sqrMagnitude); } }
        #endregion

        #region constants
        public const float epsilon = 1e-05f;
        #endregion

        #region Default Values
        public static Vec3 Zero { get { return new Vec3(0.0f, 0.0f, 0.0f); } }
        public static Vec3 One { get { return new Vec3(1.0f, 1.0f, 1.0f); } }
        public static Vec3 Forward { get { return new Vec3(0.0f, 0.0f, 1.0f); } }
        public static Vec3 Back { get { return new Vec3(0.0f, 0.0f, -1.0f); } }
        public static Vec3 Right { get { return new Vec3(1.0f, 0.0f, 0.0f); } }
        public static Vec3 Left { get { return new Vec3(-1.0f, 0.0f, 0.0f); } }
        public static Vec3 Up { get { return new Vec3(0.0f, 1.0f, 0.0f); } }
        public static Vec3 Down { get { return new Vec3(0.0f, -1.0f, 0.0f); } }
        public static Vec3 PositiveInfinity { get { return new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity); } }
        public static Vec3 NegativeInfinity { get { return new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); } }
        #endregion                                                                                                                                                                               

        #region Constructors
        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }


        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            this.x = v2.x;
            this.y = v2.y;
            this.z = 0.0f;
        }

        public static Vec3 up
        {
            get
            {
                return new Vec3(0, 1, 0);
            }
        }

        public static Vec3 right
        {
            get
            {
                return new Vec3(1, 0, 0);
            }
        }

        public static Vec3 foward
        {
            get
            {
                return new Vec3(0, 0, 1);
            }
        }
        #endregion

        #region Operators
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float diff_x = left.x - right.x;
            float diff_y = left.y - right.y;
            float diff_z = left.z - right.z;
            float sqrmag = diff_x * diff_x + diff_y * diff_y + diff_z * diff_z;
            return sqrmag < epsilon * epsilon;
        }
        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(v3.x * -1, v3.y * -1, v3.z * -1);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }
        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return new Vec3(scalar * v3.x, scalar * v3.y, scalar * v3.z);
        }
        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        public static implicit operator Vector3(Vec3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        public static implicit operator Vec3(Vector3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        public static implicit operator Vector2(Vec3 v2)
        {
            return new Vector2(v2.x, v2.y);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return "X = " + x.ToString() + "   Y = " + y.ToString() + "   Z = " + z.ToString();
        }
        public static float Angle(Vec3 from, Vec3 to)
        {
            float result = Mathf.Acos((Dot(from, to)) / (Magnitude(from) * Magnitude(to))); //devulve el valor en radianes

            return result * Mathf.Rad2Deg; //valor en grados
        }


        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            if (Magnitude(vector) > maxLength)
            {
                vector.Normalize();
                vector = vector * maxLength;

                return vector;
            }
            else
                return vector;
        }
        //Limita la magnitud de un vector a un máximo, si la magnitud del vector es mayor que el máximo lo ajusta, si es menor devuelve el mismo vector porque esta dentro del límite
        public static float Magnitude(Vec3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            float newVecX = a.y * b.z - a.z * b.y;
            float newVecY = a.z * b.x - a.x * b.z;
            float newVecZ = a.x * b.y - a.y * b.x;

            return new Vec3(newVecX, newVecY, newVecZ);
        }
        
        public static float Distance(Vec3 a, Vec3 b)
        {
            float xDiference = b.x - a.x;
            float yDiference = b.y - a.y;
            float zDiference = b.z - a.z;

            return Mathf.Sqrt(xDiference * xDiference + yDiference * yDiference + zDiference * zDiference); //Magnitud del vector de distancia
        }

        public static float Dot(Vec3 a, Vec3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z); // |A| . |B| . Cos(a)
        }
        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            t = Mathf.Clamp01(t); //Se asegura de que el factor de interpolación este si o si entre 0 y 1. Es lo mismo que hacer "Mathf.Max(0f, Mathf.Min(1f, t));"
                                  //MAXIMO devuelve el valor máximo entre 0 y el otro valor que es el MINIMO entre 1 y t. Si t es menor que 0 Max devuelve 0 y si es 
                                  //mas grande que 1 Min devuelve 1

            float newVecX = (1 - t) * a.x + t * b.x;
            float newVecY = (1 - t) * a.y + t * b.y;
            float newVecZ = (1 - t) * a.z + t * b.z;

            return new Vec3(newVecX, newVecY, newVecZ);
        }
        
        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            float newVecX = (1 - t) * a.x + t * b.x;
            float newVecY = (1 - t) * a.y + t * b.y;
            float newVecZ = (1 - t) * a.z + t * b.z;

            return new Vec3(newVecX, newVecY, newVecZ);
        }
        
        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            float maxX = (a.x > b.x) ? a.x : b.x;
            float maxY = (a.y > b.y) ? a.y : b.y;
            float maxZ = (a.z > b.z) ? a.z : b.z;

            return new Vec3(maxX, maxY, maxZ);
        }
        //Devuelve un vec3 con los componentes x, y, z mayores entre los dos vectores.
        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            float minX = (a.x < b.x) ? a.x : b.x;
            float minY = (a.y < b.y) ? a.y : b.y;
            float minZ = (a.z < b.z) ? a.z : b.z;

            return new Vec3(minX, minY, minZ);
        }
        //Devuelve un vec3 con los componentes x, y, z menores entre los dos vectores.
        public static float SqrMagnitude(Vec3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }
        public static Vec3 Project(Vec3 vector, Vec3 onNormal) 
        {
            float sqrMagnitude = SqrMagnitude(onNormal);
            float dotProduct = Dot(vector, onNormal);
            float projScalar = dotProduct / sqrMagnitude;

            return onNormal * projScalar;
        }
        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            float dotProduct = Dot(inDirection, inNormal); //mide cuánto del vector de entrada está en la dirección del vector normal.

            return inDirection - 2 * dotProduct * inNormal;
        }
        public void Set(float newX, float newY, float newZ)
        {
            x = newX; 
            y = newY; 
            z = newZ;
        }
        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }
        public void Normalize()
        {
            float vecMagnitude = Magnitude(this);

            if (vecMagnitude != 0.0f)
            {
                x /= vecMagnitude;
                y /= vecMagnitude;
                z /= vecMagnitude;
            }
        }
        #endregion

        #region Internals
        public override bool Equals(object other)
        {
            if (!(other is Vec3)) return false;
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion
    }
}