using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Parte horizontal de la T
            -1.0f,  0.25f,  0.25f,  // Vértice 0
             1.0f,  0.25f,  0.25f,  // Vértice 1
             1.0f,  0.25f, -0.25f,  // Vértice 2
            -1.0f,  0.25f, -0.25f,  // Vértice 3

            -1.0f, -0.25f,  0.25f,  // Vértice 4
             1.0f, -0.25f,  0.25f,  // Vértice 5
             1.0f, -0.25f, -0.25f,  // Vértice 6
            -1.0f, -0.25f, -0.25f,  // Vértice 7

            // Parte vertical de la T
            -0.25f,  1.0f,  0.25f,  // Vértice 8
             0.25f,  1.0f,  0.25f,  // Vértice 9
             0.25f,  0.25f,  0.25f, // Vértice 10
            -0.25f,  0.25f,  0.25f, // Vértice 11

            -0.25f,  1.0f, -0.25f,  // Vértice 12
             0.25f,  1.0f, -0.25f,  // Vértice 13
             0.25f,  0.25f, -0.25f, // Vértice 14
            -0.25f,  0.25f, -0.25f, // Vértice 15
        };

        uint[] indices = {  // note that we start from 0!
            // Parte horizontal
            0, 1, 2, 2, 3, 0,  // Cara frontal
            4, 5, 6, 6, 7, 4,  // Cara trasera
            0, 1, 5, 5, 4, 0,  // Cara superior
            3, 2, 6, 6, 7, 3,  // Cara inferior
            0, 3, 7, 7, 4, 0,  // Cara izquierda
            1, 2, 6, 6, 5, 1,  // Cara derecha

            // Parte vertical
            8, 9, 10, 10, 11, 8,  // Cara frontal
            12, 13, 14, 14, 15, 12,  // Cara trasera
            8, 9, 13, 13, 12, 8,  // Cara superior
            11, 10, 14, 14, 15, 11,  // Cara inferior
            8, 11, 15, 15, 12, 8,  // Cara izquierda
            9, 10, 14, 14, 13, 9,  // Cara derecha
        };

        private Shader shader;

        // We create a double to hold how long has passed since the program was opened.
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
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Enable variable 0 in the shader.
            //GL.EnableVertexAttribArray(0);

            // We initialize the camera so that it is 3 units back from where the rectangle is
            // and give it the proper aspect ratio
            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);

            // We make the mouse cursor invisible and captured so we can have proper FPS-camera movement
            CursorVisible = false;
            CursorGrabbed = true;

            base.OnLoad(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
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

            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
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
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += 4.0 * e.Time;

            // Limpiar el buffer de color y profundidad
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Llamada para dibujar la letra T
            //DrawT();

            GL.BindVertexArray(vertexArrayObject);

            shader.Use();

            // Finally, we have the model matrix. This determines the position of the model.
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());


            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        private void DrawT()
        {
            // Definir los vértices de la letra T en 3D
            float[] vertices = {
            // Parte horizontal de la T
            -1.0f,  0.25f,  0.25f,  // Vértice 0
             1.0f,  0.25f,  0.25f,  // Vértice 1
             1.0f,  0.25f, -0.25f,  // Vértice 2
            -1.0f,  0.25f, -0.25f,  // Vértice 3

            -1.0f, -0.25f,  0.25f,  // Vértice 4
             1.0f, -0.25f,  0.25f,  // Vértice 5
             1.0f, -0.25f, -0.25f,  // Vértice 6
            -1.0f, -0.25f, -0.25f,  // Vértice 7

            // Parte vertical de la T
            -0.25f,  1.0f,  0.25f,  // Vértice 8
             0.25f,  1.0f,  0.25f,  // Vértice 9
             0.25f,  0.25f,  0.25f, // Vértice 10
            -0.25f,  0.25f,  0.25f, // Vértice 11

            -0.25f,  1.0f, -0.25f,  // Vértice 12
             0.25f,  1.0f, -0.25f,  // Vértice 13
             0.25f,  0.25f, -0.25f, // Vértice 14
            -0.25f,  0.25f, -0.25f, // Vértice 15
        };

            // Definir los índices para los triángulos de la letra T
            uint[] indices = {
            // Parte horizontal
            0, 1, 2, 2, 3, 0,  // Cara frontal
            4, 5, 6, 6, 7, 4,  // Cara trasera
            0, 1, 5, 5, 4, 0,  // Cara superior
            3, 2, 6, 6, 7, 3,  // Cara inferior
            0, 3, 7, 7, 4, 0,  // Cara izquierda
            1, 2, 6, 6, 5, 1,  // Cara derecha

            // Parte vertical
            8, 9, 10, 10, 11, 8,  // Cara frontal
            12, 13, 14, 14, 15, 12,  // Cara trasera
            8, 9, 13, 13, 12, 8,  // Cara superior
            11, 10, 14, 14, 15, 11,  // Cara inferior
            8, 11, 15, 15, 12, 8,  // Cara izquierda
            9, 10, 14, 14, 13, 9,  // Cara derecha
        };

            // Habilitar el uso de vértices
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);

            // Dibujar la letra T usando los índices
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);

            // Deshabilitar el uso de vértices
            GL.DisableClientState(ArrayCap.VertexArray);
        }


        // This function's main purpose is to set the mouse position back to the center of the window
        // every time the mouse has moved. So the cursor doesn't end up at the edge of the window where it cannot move
        // further out
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused
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
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
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
