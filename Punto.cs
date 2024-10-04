using System;
using OpenTK;

namespace Letra_T
{
    [Serializable]
    class Punto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Punto(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Punto()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public static Punto operator -(Punto a) => new Punto(-a.X, -a.Y, -a.Z);

        public static Punto operator +(Punto a, Punto b)
            => new Punto(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Punto operator -(Punto a, Punto b)
            => new Punto(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Punto operator *(Punto a, Punto b)
            => new Punto(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Punto operator /(Punto a, Punto b)
        {
            if (b.X == 0 || b.Y == 0 || b.Z == 0)
            {
                throw new DivideByZeroException();
            }
            return new Punto(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Punto operator /(Punto a, int b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException();
            }
            return new Punto(a.X / b, a.Y / b, a.Z / b);
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);

        public float[] ToArray() => new float[] { X, Y, Z };
    }
}
