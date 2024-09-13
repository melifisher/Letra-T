using System.Collections.Generic;
using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Letra_T
{
    [Serializable]
    class Poligono
    {
        private int _vao;
        private int _vbo;
        private int _ebo;
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
            SetupBuffers();
        }
        public void Draw()
        {
            GL.Color4(Color);
            GL.Begin(PrimitiveType.Polygon);
            foreach( var punto in Puntos)
            {
                GL.Vertex3(punto.X, punto.Y, punto.Z);
            }
            GL.End();
        }

        private void CalculateCenterOfMass()
        {
            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var vertex in Puntos)
            {
                sumX += vertex.X;
                sumY += vertex.Y;
                sumZ += vertex.Z;
            }
            int count = Puntos.Count;
            CenterOfMass = new Punto(sumX / count, sumY / count, sumZ / count);
        }

        private void SetupBuffers()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, GetVertexData().Length * sizeof(float), GetVertexData(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);


            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }

        public float[] GetVertexData()
        {
            var vertexData = new List<float>();
            foreach (var vertex in Puntos)
            {
                vertexData.AddRange(vertex.ToArray());
                vertexData.AddRange(new[] { Color.R / 255f, Color.G / 255f, Color.B / 255f, 1.0f });
            }
            return vertexData.ToArray();
        }
    }
}
