using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace Letra_T
{
    public class Game : GameWindow
    {
        private Shader shader;
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        private Escenario escenario;

        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _camera = new Camera(Vector3.UnitZ * 2, Width / (float)Height);
            CursorVisible = false;
            CursorGrabbed = true;

            //var letraT = CrearLetraT();
            //ObjetoSerializer.GuardarObjeto(letraT, "letraT.json");

            escenario = new Escenario(shader, _camera);
            escenario.AddObjeto("letraT", CargarLetraT());

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

            CameraUpdate(input, (float)e.Time);

            escenario.Update((float)e.Time);

            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            escenario.Render();
            //escenario.Draw();

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        private void CameraUpdate(KeyboardState input, float deltaTime)
        {
            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Front * cameraSpeed * deltaTime; // Forward
            }

            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Front * cameraSpeed * deltaTime; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * deltaTime; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * deltaTime; // Right
            }
            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Up * cameraSpeed * deltaTime; // Up
            }
            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Up * cameraSpeed * deltaTime; // Down
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

            //foreach (var escenario in Escenarios)
            //{
                escenario.Camera.AspectRatio = Width / (float)Height;
            //}
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            escenario.Dispose();
            GL.DeleteProgram(shader.Handle);

            base.OnUnload(e);
        }
        private Objeto CargarLetraT()
        {
            return ObjetoSerializer.Cargar<Objeto>("letraT.json");
        }
            private Objeto CrearLetraT()
        {
            var letraT = new Objeto();

            var parteVertical = new Parte();

            // Cara frontal (roja)
            parteVertical.AddPoligono("rojo",new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f,  0.1f),
            new Punto( 0.1f, -0.5f,  0.1f),
            new Punto( 0.1f,  0.0f,  0.1f),
            new Punto(-0.1f,  0.0f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Red
            ));

            // Cara trasera (verde)
            parteVertical.AddPoligono("verde",new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f, -0.1f),
            new Punto( 0.1f,  0.0f, -0.1f),
            new Punto(-0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Green
            ));

            // Cara izquierda (azul)
            parteVertical.AddPoligono("azul",new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto(-0.1f, -0.5f,  0.1f),
            new Punto(-0.1f,  0.0f,  0.1f),
            new Punto(-0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Blue
            ));

            // Cara derecha (amarilla)
            parteVertical.AddPoligono("amarillo",new Poligono(
                new List<Punto> {
            new Punto(0.1f, -0.5f, -0.1f),
            new Punto(0.1f, -0.5f,  0.1f),
            new Punto(0.1f,  0.0f,  0.1f),
            new Punto(0.1f,  0.0f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Yellow
            ));

            // Cara inferior (cian)
            parteVertical.AddPoligono("cian",new Poligono(
                new List<Punto> {
            new Punto(-0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f, -0.1f),
            new Punto( 0.1f, -0.5f,  0.1f),
            new Punto(-0.1f, -0.5f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Cyan
            ));

            // Parte horizontal de la T
            var parteHorizontal = new Parte();
            parteHorizontal.CenterOfMass = new Punto(0, 0.05f, 0); 

            // Cara frontal (magenta)
            parteHorizontal.AddPoligono("magenta", new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f,  0.1f),
            new Punto( 0.5f,  0.0f,  0.1f),
            new Punto( 0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Magenta
            ));

            // Cara trasera (naranja)
            parteHorizontal.AddPoligono("naranja",new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.1f, -0.1f),
            new Punto(-0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Orange
            ));

            // Cara superior (blanca)
            parteHorizontal.AddPoligono("blanca",new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.1f, -0.1f),
            new Punto( 0.5f,  0.1f, -0.1f),
            new Punto( 0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.White
            ));

            // Cara inferior (gris)
            parteHorizontal.AddPoligono("gris",new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f, -0.1f),
            new Punto( 0.5f,  0.0f,  0.1f),
            new Punto(-0.5f,  0.0f,  0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Gray
            ));

            // Cara izquierda (púrpura)
            parteHorizontal.AddPoligono("púrpura",new Poligono(
                new List<Punto> {
            new Punto(-0.5f,  0.0f, -0.1f),
            new Punto(-0.5f,  0.0f,  0.1f),
            new Punto(-0.5f,  0.1f,  0.1f),
            new Punto(-0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 1, 2, 2, 3, 0 },
                Color.Purple
            ));

            // Cara derecha (lima)
            parteHorizontal.AddPoligono("lima",new Poligono(
                new List<Punto> {
            new Punto(0.5f,  0.0f, -0.1f),
            new Punto(0.5f,  0.0f,  0.1f),
            new Punto(0.5f,  0.1f,  0.1f),
            new Punto(0.5f,  0.1f, -0.1f)
                },
                new uint[] { 0, 3, 2, 2, 1, 0 },
                Color.Lime
            ));

            letraT.AddParte("vertical",parteVertical);
            letraT.AddParte("horizontal",parteHorizontal);

            return letraT;
        }
    }
}

