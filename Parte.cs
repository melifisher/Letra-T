using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Letra_T
{
    [Serializable]
    class Parte
    {
        public Dictionary<string,Poligono> Poligonos { get; set; }
        public Punto CenterOfMass { get; set; }
        private Vector3 Position { get; set; }
        private Vector3 Rotation { get; set; }
        private float scaleSpeed = 0.01f;
        private Vector3 Scale { get; set; }

        public Parte()
        {
            Poligonos = new Dictionary<string, Poligono>();
            RecalculateCenterOfMass();
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }
        public Parte(Punto centerOfMass)
        {
            Poligonos = new Dictionary<string, Poligono>();
            CenterOfMass = centerOfMass;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public void AddPoligono(string key, Poligono poligono)
        {
            Poligonos.Add(key, poligono);
            RecalculateCenterOfMass();
        }
        public Poligono GetPoligono(string key)
        {
            return Poligonos[key];
        }
        public void DeletePoligono(string key)
        {
            Poligonos.Remove(key);
        }

        public void Draw()
        {
            //Matrix4 parteModelMatrix = GetModelMatrix() * objetoModelMatrix;
            //GL.MultMatrix(ref parteModelMatrix);

            foreach (var poligono in Poligonos)
            {
                poligono.Value.Draw();
            }
        }
        public void Update(float deltaTime)
        {
        }
        private void RecalculateCenterOfMass()
        {
            if (Poligonos.Count == 0)
            {
                CenterOfMass = new Punto();
                return;
            }

            float sumX = 0, sumY = 0, sumZ = 0;
            foreach (var poligono in Poligonos)
            {
                sumX += poligono.Value.CenterOfMass.X;
                sumY += poligono.Value.CenterOfMass.Y;
                sumZ += poligono.Value.CenterOfMass.Z;
            }
            int count = Poligonos.Count;
            CenterOfMass = new Punto(sumX / count, sumY / count, sumZ / count);
        }

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateTranslation(-CenterOfMass.ToVector3())
                * Matrix4.CreateScale(Scale)
                * Matrix4.CreateRotationX(Rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y)
                * Matrix4.CreateRotationZ(Rotation.Z)
                * Matrix4.CreateTranslation(CenterOfMass.ToVector3())
                * Matrix4.CreateTranslation(Position);
        }
    }
}
