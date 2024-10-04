using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using OpenTK.Input;

namespace Letra_T
{
    [Serializable]
    class Objeto : Transformable
    {
        public Dictionary<string, Parte> Partes { get; set; }
        private readonly bool recalculate;
        public Objeto()
        {
            Partes = new Dictionary<string, Parte>();
            CenterOfMass = new Punto();
            recalculate = true;
        }
        public Objeto(Punto centerOfMass)
        {
            Partes = new Dictionary<string, Parte>();
            RecalculateCenterOfMass();
            recalculate = false;
        }

        public void AddParte(string key, Parte parte)
        {
            Partes.Add(key, parte);
            if(recalculate) RecalculateCenterOfMass();
        }
        public Parte GetParte(string key)
        {
            return Partes[key];
        }
        public void DeleteParte(string key)
        {
            Partes.Remove(key);
        }
        override
        public void Draw()
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transformMatrix);

            foreach (var parte in Partes)
            {
                parte.Value.Draw();
            }
            GL.PopMatrix();
        }

        private void RecalculateCenterOfMass()
        {
            if (Partes.Count == 0)
            {
                CenterOfMass = new Punto();
                return;
            }

            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var parte in Partes)
            {
                sumX += parte.Value.CenterOfMass.X;
                sumY += parte.Value.CenterOfMass.Y;
                sumZ += parte.Value.CenterOfMass.Z;
            }
            int count = Partes.Count;
            CenterOfMass = new Punto(sumX / count, sumY / count, sumZ / count);
        }
        public override bool Intersects(Vector3 rayOrigin, Vector3 rayDirection, out float distance)
        {
            distance = float.MaxValue;
            bool hit = false;

            foreach (var parte in Partes)
            {
                float parteDistance;
                if (parte.Value.Intersects(rayOrigin, rayDirection, out parteDistance))
                {
                    if (parteDistance < distance)
                    {
                        distance = parteDistance;
                        hit = true;
                    }
                }
            }

            return hit;
        }

        //public void Update(float deltaTime)
        //{
        //    Rotation += new Vector3(deltaTime, deltaTime, 0);

        //    var keyboard = Keyboard.GetState();

        //    if (keyboard.IsKeyDown(Key.Up))
        //    {
        //        Scale += new Vector3(0.01f);
        //    }
        //    if (keyboard.IsKeyDown(Key.Down))
        //    {
        //        Scale -= new Vector3(0.01f);
        //        Scale = new Vector3(Math.Max(Scale.X, 0.1f), Math.Max(Scale.Y, 0.1f), Math.Max(Scale.Z, 0.1f));
        //    }

        //    if (keyboard.IsKeyDown(Key.Right))
        //    {
        //        Position += new Vector3(0.01f, 0, 0);
        //    }
        //    if (keyboard.IsKeyDown(Key.Left))
        //    {
        //        Position -= new Vector3(0.01f, 0, 0);
        //    }

        //    foreach (var parte in Partes)
        //    {
        //        parte.Value.Update(deltaTime);
        //    }
        //    //Partes["vertical"].Update(deltaTime);
        //}
        //public Matrix4 GetModelMatrix()
        //{
        //    return Matrix4.CreateTranslation(-CenterOfMass.ToVector3())
        //        * Matrix4.CreateScale(Scale)
        //        * Matrix4.CreateRotationX(Rotation.X)
        //        * Matrix4.CreateRotationY(Rotation.Y)
        //        * Matrix4.CreateRotationZ(Rotation.Z)
        //        * Matrix4.CreateTranslation(CenterOfMass.ToVector3())
        //        * Matrix4.CreateTranslation(Position);
        //}
        //public Matrix4 GetPartesModelMatrix()
        //{
        //    Matrix4 parteModelMatrix = Matrix4.Identity;
        //    foreach (var parte in Partes)
        //    {
        //        parteModelMatrix *= parte.Value.GetModelMatrix();
        //    }
        //    return parteModelMatrix;
        //}

    }
}
