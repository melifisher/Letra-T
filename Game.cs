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
        private Escenario escenario;
        private Vector3 cameraPosition = Vector3.UnitZ * 2;
        private float cameraSpeed = 0.1f;

        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            //CursorVisible = true;

            //var letraT = CrearLetraT();
            //ObjetoSerializer.Guardar<Objeto>(letraT, "letraT.json");

            escenario = new Escenario();
            escenario.AddObjeto("letraT", CargarLetraT());

            base.OnLoad(e);
        }
        private Objeto CargarLetraT()
        {
            return ObjetoSerializer.Cargar<Objeto>("letraT.json");
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            CameraUpdate(input);

            escenario.Update((float)e.Time);

            base.OnUpdateFrame(e);
        }

        private void CameraUpdate(KeyboardState input)
        {
            if (input.IsKeyDown(Key.W))
                cameraPosition.Z -= cameraSpeed;
            if (input.IsKeyDown(Key.S))
                cameraPosition.Z += cameraSpeed;
            if (input.IsKeyDown(Key.A))
                cameraPosition.X -= cameraSpeed;
            if (input.IsKeyDown(Key.D))
                cameraPosition.X += cameraSpeed;
            if (input.IsKeyDown(Key.Q))
                cameraPosition.Y += cameraSpeed;
            if (input.IsKeyDown(Key.E))
                cameraPosition.Y -= cameraSpeed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 viewMatrix = Matrix4.LookAt(cameraPosition, cameraPosition - Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMatrix);

            escenario.Draw();

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            UpdateProjectionMatrix();
        }

        private void UpdateProjectionMatrix()
        {
            float aspect = (float)Width / Height;
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
        
            private Objeto CrearLetraT()
        {
            var letraT = new Objeto(new Punto(-0.1f, -0.5f, 0.1f));

            // Parte vertical de la T
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

