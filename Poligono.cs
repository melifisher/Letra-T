using System.Collections.Generic;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Letra_T
{
    [Serializable]
    class Poligono
    {
        public List<Punto> Puntos { get; set; }
        public uint[] Indices { get; private set; }
        public Color Color { get; set; }
        public Punto CenterOfMass { get; set; }

        public Poligono(List<Punto> puntos, uint[] indices, Color color)
        {
            Puntos = puntos;
            Indices = indices;
            Color = color; 
            CalculateCenterOfMass();
        }
        public void Draw()
        {
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color);
            foreach (var punto in Puntos)
            {
                GL.Vertex3(punto.X, punto.Y, punto.Z);
            }
            GL.End();
        }
        private void CalculateCenterOfMass()
        {
            Punto sum = new Punto();
            foreach (var vertex in Puntos)
            {
                sum += vertex;
            }
            CenterOfMass = sum/ Puntos.Count;
        }

    }
}
