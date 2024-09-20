using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using System;

namespace Letra_T
{
    class Escenario : Transformable
    {
        public Dictionary<string, Objeto> Objetos { get; private set; }
        public Escenario()
        {
            Objetos = new Dictionary<string, Objeto>();
            CenterOfMass = new Punto();
        }
        public void AddObjeto(string key, Objeto objeto)
        {
            Objetos.Add(key, objeto);
        }
        public Objeto GetObjeto(string key)
        {
            return Objetos[key];
        }
        public void DeleteObjeto(string key)
        {
            Objetos.Remove(key);
        }
        
        public override void Draw()
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transformMatrix);
            
            DrawReferenceAxes();

            foreach (var objeto in Objetos)
            {
                objeto.Value.Draw();
            }

            GL.PopMatrix();
        }

        private void DrawReferenceAxes()
        {
            GL.LineWidth(2.0f);
            GL.Begin(PrimitiveType.Lines);

            // Eje X (rojo)
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);

            // Eje Y (verde)
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 1, 0);

            // Eje Z (azul)
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 1);

            GL.End();
            GL.LineWidth(1.0f);
        }

        //public void Update(float deltaTime)
        //{
        //    Objeto obj = Objetos["letraT1"];
        //    //Rotation += new Vector3(deltaTime, deltaTime, 0);
        //    //Parte part = obj.Partes["horizontal"];
        //    //part.Rotar(new Vector3(0, MathHelper.DegreesToRadians(1), 0));

        //    var keyboard = Keyboard.GetState();

        //    if (keyboard.IsKeyDown(Key.Up))
        //    { 
        //        //Scale += new Vector3(0.01f);
        //        obj.Escalar(new Vector3(0.01f));
        //    }
        //    if (keyboard.IsKeyDown(Key.Down))
        //    {
        //        //Scale =- new Vector3(0.01f);
        //        //Scale = new Vector3(Math.Max(Scale.X, 0.1f), Math.Max(Scale.Y, 0.1f), Math.Max(Scale.Z, 0.1f));
        //        obj.Escalar(-new Vector3(0.01f));
        //    }

        //    if (keyboard.IsKeyDown(Key.Right))
        //    {
        //        //Position += new Vector3(0.01f, 0, 0);
        //        obj.Trasladar(new Vector3(0.01f, 0, 0));
        //    }
        //    if (keyboard.IsKeyDown(Key.Left))
        //    {
        //        //Position -= new Vector3(0.01f, 0, 0);
        //        obj.Trasladar(-new Vector3(0.01f, 0, 0));
        //    }

        //    //foreach (var objeto in Objetos)
        //    //{
        //    //    objeto.Value.Update(deltaTime);
        //    //}
        //}
    }
}