using System.Collections.Generic;
using OpenTK;


namespace Letra_T
{
    public class Objeto
    {
        public Dictionary<string, Parte> Partes { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 CenterOfMass { get; private set; }

        public Objeto()
        {
            Partes = new Dictionary<string, Parte>();
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            CenterOfMass = Vector3.Zero;
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

        private void RecalculateCenterOfMass()
        {
            if (Partes.Count == 0)
            {
                CenterOfMass = Vector3.Zero;
                return;
            }

            Vector3 sum = Vector3.Zero;
            foreach (var parte in Partes)
            {
                sum += parte.Value.CenterOfMass + parte.Value.Position;
                //sum += parte.Value.CenterOfMass;
            }
            CenterOfMass = sum / Partes.Count;
        }

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateTranslation(-CenterOfMass)
                 * Matrix4.CreateScale(Scale)
                 * Matrix4.CreateRotationX(Rotation.X)
                 * Matrix4.CreateRotationY(Rotation.Y)
                 * Matrix4.CreateRotationZ(Rotation.Z)
                 * Matrix4.CreateTranslation(CenterOfMass)
                 *Matrix4.CreateTranslation(Position);
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
