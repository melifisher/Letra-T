using System;

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

        public float[] ToArray()
        {
            return new float[] { X, Y, Z };
        }
    }
}
