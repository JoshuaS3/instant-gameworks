using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace InstantGameworks.Math
{
    public struct Vector2
    {
        public static readonly Vector2 UnitX = new Vector2(1, 0);
        public static readonly Vector2 UnitY = new Vector2(0, 1);
        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector2());

        public static Vector2 Add(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2 Subtract(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }
        public static Vector2 Multiply(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X * right.X, left.Y * right.Y);
        }
        public static Vector2 Divide(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X / right.X, left.Y / right.Y);
        }
        public static Vector2 Min(Vector2 left, Vector2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }
        public static Vector2 Max(Vector2 left, Vector2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }
        public static Vector2 Clamp(Vector2 vec, Vector2 left, Vector2 right)
        {
            vec.X = (vec.X < left.X ? left.X : vec.X) > right.X ? right.X : vec.X;
            vec.Y = (vec.Y < left.Y ? left.Y : vec.Y) > right.Y ? right.Y : vec.Y;
            return vec;
        }
        public static float Dot(Vector2 left, Vector2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return Add(left, right);
        }
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return Subtract(left, right);
        }
        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return Multiply(left, right);
        }
        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return Divide(left, right);
        }
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }


        public float X { get; set; }
        public float Y { get; set; }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }
        public float LengthSquared
        {
            get
            {
                return (X * X + Y * Y);
            }
        }
        public Vector2 PerpendicularRight
        {
            get
            {
                return new Vector2(Y, -X);
            }
        }
        public Vector2 PerpendicularLeft
        {
            get
            {
                return new Vector2(-Y, X);
            }
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2 Add(Vector2 second)
        {
            X += second.X;
            Y += second.Y;
            return this;
        }


        public Vector2 Subtract(Vector2 second)
        {
            X -= second.X;
            Y -= second.Y;
            return this;
        }


        public Vector2 Multiply(float scalar)
        {
            X *= scalar;
            Y *= scalar;
            return this;
        }
        public Vector2 Multiply(Vector2 second)
        {
            X *= second.X;
            Y *= second.Y;
            return this;
        }

        public Vector2 Divide(float scalar)
        {
            X /= scalar;
            Y /= scalar;
            return this;
        }
        public Vector2 Divide(Vector2 second)
        {
            X /= second.X;
            Y /= second.Y;
            return this;
        }
            

        public Vector2 Scale(float x, float y)
        {
            X *= x;
            Y *= y;
            return this;
        }
        public Vector2 Scale(Vector2 scale)
        {
            X *= scale.X;
            Y *= scale.Y;
            return this;
        }



        public Vector2 Normalize()
        {
            float distance = (float)Math.Sqrt((X * X) + (Y * Y));
            X = X / distance;
            Y = Y / distance;
            return this;
        }

        public override string ToString()
        {
            return string.Concat("(", X, ", ", Y, ")");
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vector2)) return false;
            return Equals((Vector2)obj);
        }
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}
