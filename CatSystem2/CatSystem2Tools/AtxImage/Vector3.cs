using System;
using System.Collections;
using System.Collections.Generic;


namespace AtxImage
{
    /// <summary>
    /// double version of unity vector3
    /// </summary>
    public class Vector3
    {

        /// <summary>
        /// x component of vector
        /// </summary>
        public float x;

        /// <summary>
        /// y component of vector
        /// </summary>
        public float y;

        /// <summary>
        /// z component of vector
        /// </summary>
        public float z;


        #region constructor
        public Vector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vector3(float p_x, float p_y)
        {
            x = p_x;
            y = p_y;
            z = 0;
        }

        public Vector3(float p_x, float p_y, float p_z)
        {
            x = p_x;
            y = p_y;
            z = p_z;
        }

        #endregion

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid vector3d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        {
                            x = value;
                            break;
                        }
                    case 1:
                        {
                            y = value;
                            break;
                        }
                    case 2:
                        {
                            z = value;
                            break;
                        }
                    default:
                        throw new IndexOutOfRangeException("Invalid vector3d index!");
                }
            }
        }

        public static Vector3 zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }

        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt(sqrMagnitude);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public Vector3 normalized
        {
            get
            {
                return Normalize(this);
            }
        }


        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            float x = lhs.y * rhs.z - rhs.y * lhs.z;
            float y = rhs.x * lhs.z - lhs.x * rhs.z;
            float z = lhs.x * rhs.y - rhs.x - lhs.y;
            return new Vector3(x, y, z);

        }

        /// <summary>
        /// 两点距离
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Distance(Vector3 s, Vector3 t)
        {
            return (s - t).magnitude;
        }

        /// <summary>
        /// 单位化
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 Normalize(Vector3 vec)
        {
            if (vec == zero)
                return zero;
            else
            {
                Vector3 res = new Vector3();
                res.x = vec.x / vec.magnitude;
                res.y = vec.y / vec.magnitude;
                res.z = vec.z / vec.magnitude;
                return res;
            }
        }

        /// <summary>
        /// 单位化
        /// </summary>
        public void Normalize()
        {
            if (this != zero)
            {
                float length = magnitude;
                x /= length;
                y /= length;
                z /= length;
            }
        }

        /// <summary>
        /// 模长平方
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double SqrMagnitude(Vector3 a)
        {
            return a.sqrMagnitude;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        #region override
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }
        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));
        }
        public override bool Equals(object other)
        {
            return this == (Vector3)other;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }

        #endregion

        #region operator
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(d * a.x, d * a.y, d * a.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(d * a.x, d * a.y, d * a.z);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            if (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        #endregion



    }
}