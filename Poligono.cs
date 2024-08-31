using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Letra_T
{

    public class Poligono
    {
        private int _vao;
        private int _vbo;
        private int _ebo;
        public Vector3[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public Color Color { get; set; }
        public Vector3 CenterOfMass { get; set; }

        public Poligono(Vector3[] vertices, uint[] indices, Color color)
        {
            Vertices = vertices;
            Indices = indices;
            Color = color; 
            CalculateCenterOfMass();
            SetupBuffers();
        }
        private void CalculateCenterOfMass()
        {
            Vector3 sum = Vector3.Zero;
            foreach (var vertex in Vertices)
            {
                sum += vertex;
            }
            CenterOfMass = sum / Vertices.Length;
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
            foreach (var vertex in Vertices)
            {
                vertexData.AddRange(new[] { vertex.X, vertex.Y, vertex.Z });
                vertexData.AddRange(new[] { Color.R / 255f, Color.G / 255f, Color.B / 255f, 1.0f });
            }
            return vertexData.ToArray();
        }
    }
}
