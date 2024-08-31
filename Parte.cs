using System;
using System.Collections.Generic;
using OpenTK;


namespace Letra_T
{
    public class Parte
    {
        public Dictionary<string,Poligono> Poligonos { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 CenterOfMass { get; private set; }

        public Parte()
        {
            Poligonos = new Dictionary<string, Poligono>();
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            CenterOfMass = Vector3.Zero;
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
        private void RecalculateCenterOfMass()
        {
            if (Poligonos.Count == 0)
            {
                CenterOfMass = Vector3.Zero;
                return;
            }

            Vector3 sum = Vector3.Zero;
            foreach (var poligono in Poligonos)
            {
                sum += poligono.Value.CenterOfMass;
            }
            CenterOfMass = sum / Poligonos.Count;
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
            foreach (var poligono in Poligonos)
            {
                poligono.Value.Dispose();
            }
        }
    }
}
