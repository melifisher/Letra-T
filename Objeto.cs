using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Letra_T
{
    [Serializable]
    class Objeto
    {
        public Dictionary<string, Parte> Partes { get; set; }
        public Punto CenterOfMass { get; set; }

        public Objeto()
        {
            Partes = new Dictionary<string, Parte>();
            CenterOfMass = new Punto();
        }

        public void AddParte(string key, Parte parte)
        {
            Partes.Add(key, parte);
            RecalculateCenterOfMass();
        }
        public Parte GetParte(string key)
        {
            return Partes[key];
        }
        public void DeleteParte(string key)
        {
            Partes.Remove(key);
        }
        public void Draw()
        {
            foreach (var parte in Partes)
            {
                parte.Value.Draw();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var parte in Partes)
            {
                parte.Value.Update(deltaTime);
            }

            //Rotation += new Vector3(0, deltaTime, 0);
            GL.Rotate(deltaTime * 5.0, new Vector3d(-1, 1, 0));
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
                //sum += parte.Value.CenterOfMass + parte.Value.Position;
                sumX += parte.Value.CenterOfMass.X;
                sumY += parte.Value.CenterOfMass.Y;
                sumZ += parte.Value.CenterOfMass.Z;
            }
            int count = Partes.Count;
            CenterOfMass = new Punto(sumX / count, sumY / count, sumZ / count);
        }

        public Matrix4 GetModelMatrix()
        {
            Vector3 CenterOfMassVector = new Vector3(CenterOfMass.X, CenterOfMass.Y, CenterOfMass.Z);

            return Matrix4.CreateTranslation(-CenterOfMassVector)
                 * Matrix4.CreateTranslation(CenterOfMassVector);
        }
        
        public void Dispose()
        {
            foreach (var parte in Partes)
            {
                parte.Value.Dispose();   
            }
        }
    }
}
