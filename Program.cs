using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace Letra_T
{
    public class Game : GameWindow
    {
        private int vertexBufferObject;
        private int vertexArrayObject;
        private int ElementBufferObject;

        float[] vertices = {
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

        uint[] indices = {
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

        private Shader shader;

        private double _time;

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;


        public Game(int width, int height, string title) 
            : base(width, height, GraphicsMode.Default, title) 
        { 
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest); // Habilitar prueba de profundidad para 3D

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
       
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Bind the VAO
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPosition");
            var vertexColor = shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.EnableVertexAttribArray(vertexColor);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(vertexColor, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));


            _camera = new Camera(Vector3.UnitZ * 1, Width / (float)Height);

            CursorVisible = false;
            CursorGrabbed = true;

            base.OnLoad(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused)
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Estado del mouse
            var mouse = Mouse.GetState();

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calcula el offset de la pos del mouse
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }

            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += 5.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(vertexArrayObject);

            shader.Use();

            var model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());


            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            
            _camera.AspectRatio = Width / (float)Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);

            GL.DeleteProgram(shader.Handle);

            base.OnUnload(e);
        }

        static void Main(string[] args)
        {
            using (Game game = new Game(800, 600, "Letra T"))
            {
                game.Run(60.0);
            }
        }
    }
}
