using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Letra_T
{
    [Serializable]
    class Parte : Transformable
    {
        public Dictionary<string,Poligono> Poligonos { get; set; }
        private readonly bool recalculate;
        public Parte()
        {
            Poligonos = new Dictionary<string, Poligono>();
            RecalculateCenterOfMass();
            recalculate = true;
        }
        public Parte(Punto centerOfMass)
        {
            Poligonos = new Dictionary<string, Poligono>();
            CenterOfMass = centerOfMass;
            recalculate = false;
        }

        public void AddPoligono(string key, Poligono poligono)
        {
            Poligonos.Add(key, poligono);
            if(recalculate) RecalculateCenterOfMass();
        }
        public Poligono GetPoligono(string key)
        {
            return Poligonos[key];
        }
        public void DeletePoligono(string key)
        {
            Poligonos.Remove(key);
        }
        
        public override void Draw()
        {
            GL.PushMatrix();
            GL.MultMatrix(ref transformMatrix);

            foreach (var poligono in Poligonos)
            {
                poligono.Value.Draw();
            }

            GL.PopMatrix();
        }

        public override bool Intersects(Vector3 rayOrigin, Vector3 rayDirection, out float distance)
        {
            return base.Intersects(rayOrigin, rayDirection, out distance);
        }

        private void RecalculateCenterOfMass()
        {
            if (Poligonos.Count == 0)
            {
                CenterOfMass = new Punto();
                return;
            }

            Punto sum = new Punto();
            foreach (var poligono in Poligonos)
            {
                sum += poligono.Value.CenterOfMass;
            }
            CenterOfMass = sum / Poligonos.Count;
        }
    }
}
