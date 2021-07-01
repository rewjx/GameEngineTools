using System;
using System.Collections;
using System.Collections.Generic;



namespace AtxImage
{
    /// <summary>
    /// double version of unity vector3
    /// </summary>
    public class Vector2Int
    {

        /// <summary>
        /// x component of vector
        /// </summary>
        public int x;

        /// <summary>
        /// y component of vector
        /// </summary>
        public int y;



        #region constructor
        public Vector2Int()
        {
            x = 0;
            y = 0;
        }

        public Vector2Int(int p_x, int p_y)
        {
            x = p_x;
            y = p_y;
        }

        #endregion

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
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
                    default:
                        throw new IndexOutOfRangeException("Invalid vector3d index!");
                }
            }
        }

        public static Vector2Int zero
        {
            get
            {
                return new Vector2Int(0, 0);
            }
        }

        public double magnitude
        {
            get
            {
                return Math.Sqrt(sqrMagnitude);
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }


        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static int Dot(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static int Cross(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x * rhs.y - lhs.y * rhs.x;

        }

        /// <summary>
        /// 两点距离
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Distance(Vector2Int s, Vector2Int t)
        {
            return (s - t).magnitude;
        }


        /// <summary>
        /// 模长平方
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double SqrMagnitude(Vector2Int a)
        {
            return a.sqrMagnitude;
        }


        #region override
        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }
        public string ToString(string format)
        {
            return String.Format("({0}, {1})", x.ToString(format), y.ToString(format));
        }
        public override bool Equals(object other)
        {
            return this == (Vector2Int)other;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() >> 2;
        }

        #endregion

        #region operator
        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a)
        {
            return new Vector2Int(-a.x, -a.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static Vector2Int operator *(int d, Vector2Int a)
        {
            return new Vector2Int(d * a.x, d * a.y);
        }

        public static Vector2Int operator *(Vector2Int a, int d)
        {
            return new Vector2Int(d * a.x, d * a.y);
        }


        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            if (lhs.x == rhs.x && lhs.y == rhs.y)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs == rhs);
        }
        #endregion



    }
}