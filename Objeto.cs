using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Letra_T
{
    class Objeto
    {
        private int _vbo;
        private int _vao;
        private int _ebo;
        private Shader _shader;

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 CenterOfMass { get; set; }

        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        public Objeto()
        {
            InitializeVertices();
            InitializeIndices();
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            CenterOfMass = new Vector3(0, -0.2f, 0); // Aproximación del centro geométrico
        }
        private void InitializeVertices()
        {
            Vertices = new float[]
            {
                // Bottom part of T (vertical stem)
                // Front face (rojo)
                -0.1f, -0.5f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Bottom left
                 0.1f, -0.5f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Bottom right
                 0.1f,  0.0f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Top right
                -0.1f,  0.0f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Top left
                // Back face (verde)
                -0.1f, -0.5f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Bottom left
                 0.1f, -0.5f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Bottom right
                 0.1f,  0.0f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Top right
                -0.1f,  0.0f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Top left

                // Top part of T (horizontal bar)
                // Front face (rojo)
                -0.5f,  0.0f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Bottom left
                 0.5f,  0.0f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Bottom right
                 0.5f,  0.1f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Top right
                -0.5f,  0.1f,  0.1f,  1.0f, 0.0f, 0.0f, 1f,  // Top left
                // Back face (verde)
                -0.5f,  0.0f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Bottom left
                 0.5f,  0.0f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Bottom right
                 0.5f,  0.1f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Top right
                -0.5f,  0.1f, -0.1f,  0.0f, 1.0f, 0.0f, 1f,  // Top left
            };
        }
        private void InitializeIndices()
        {
            Indices = new uint[]
            {
                // Bottom part of T (vertical stem)
                0, 1, 2, 2, 3, 0, // Front face
                4, 5, 6, 6, 7, 4, // Back face
                0, 3, 7, 7, 4, 0, // Left face
                1, 5, 6, 6, 2, 1, // Right face
                3, 2, 6, 6, 7, 3, // Top face
                0, 4, 5, 5, 1, 0, // Bottom face

                // Top part of T (horizontal bar)
                8, 9, 10, 10, 11, 8, // Front face
                12, 13, 14, 14, 15, 12, // Back face
                8, 11, 15, 15, 12, 8, // Left face
                9, 13, 14, 14, 10, 9, // Right face
                11, 10, 14, 14, 15, 11, // Top face
                8, 12, 13, 13, 9, 8 // Bottom face
            };
         }
        public void Load(Shader shader)
        {
            _shader = shader;

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            var vertexColor = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.EnableVertexAttribArray(vertexColor);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(vertexColor, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            GL.BindVertexArray(_vao);
            _shader.Use();

            Matrix4 model = Matrix4.CreateTranslation(-CenterOfMass)
                            *Matrix4.CreateScale(Scale)
                            * Matrix4.CreateRotationX(Rotation.X)
                            * Matrix4.CreateRotationY(Rotation.Y)
                            * Matrix4.CreateRotationZ(Rotation.Z)
                            * Matrix4.CreateTranslation(CenterOfMass)
                            * Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Unload()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
