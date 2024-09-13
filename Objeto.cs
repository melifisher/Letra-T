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
        public Punto Position { get; set; }
        public Punto Rotation { get; set; }
        public Punto Scale { get; set; }

        public Objeto()
        {
            Partes = new Dictionary<string, Parte>();
            Position = new Punto();
            Rotation = new Punto();
            Scale = new Punto(1, 1, 1);
            CenterOfMass = new Punto();
        }

        public Objeto(Punto CenterOfMass)
        {
            Partes = new Dictionary<string, Parte>();
            Position = new Punto();
            Rotation = new Punto();
            Scale = new Punto(1, 1, 1);
            this.CenterOfMass = CenterOfMass;
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

            Rotation += new Punto(0, deltaTime, 0);
            //GL.Rotate(deltaTime * 5.0, new Vector3d(-1, 1, 0));
        }
        private void RecalculateCenterOfMass()
        {
            if (Partes.Count == 0)
            {
                CenterOfMass = new Punto();
                return;
            }

            Punto sum = new Punto();
            foreach (var parte in Partes)
            {
                sum += parte.Value.CenterOfMass + parte.Value.Position;
            }
            int count = Partes.Count;
            CenterOfMass = sum / count;
        }

        public Matrix4 GetModelMatrix()
        {
            Vector3 CenterOfMassVector = new Vector3(CenterOfMass.X, CenterOfMass.Y, CenterOfMass.Z);
            Vector3 ScaleVector = new Vector3(Scale.X, Scale.Y, Scale.Z);
            Vector3 PositionVector = new Vector3(Position.X, Position.Y, Position.Z);

            return Matrix4.CreateTranslation(-CenterOfMassVector)
                 * Matrix4.CreateScale(ScaleVector)
                * Matrix4.CreateRotationX(Rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y)
                * Matrix4.CreateRotationZ(Rotation.Z)
                * Matrix4.CreateTranslation(CenterOfMassVector)
                * Matrix4.CreateTranslation(PositionVector);
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
