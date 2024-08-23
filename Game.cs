using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace Letra_T
{
    public class Game : GameWindow
    {
        private Objeto _objeto;
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
            
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _objeto = new Objeto();
            _objeto.Load(shader);

            // Posicionar la letra T
            _objeto.Position = new Vector3(0, 0, -2);
            // Rotar la letra T
            _objeto.Rotation = new Vector3(0, MathHelper.DegreesToRadians(-30), 0);
            // Escalar la letra T
            _objeto.Scale = new Vector3(1f, 1f, 1f);
            //_objeto.CenterOfMass = new Vector3(0, -0.25f, 0);

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

            _objeto.Rotation += new Vector3(0, 0, (float)e.Time);

            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += 5.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _objeto.Render( _camera.GetViewMatrix(), _camera.GetProjectionMatrix());

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
            _objeto.Unload();
            GL.DeleteProgram(shader.Handle);

            base.OnUnload(e);
        }
    }
}

