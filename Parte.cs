﻿using System;
using System.Collections.Generic;
using OpenTK;


namespace Letra_T
{
    [Serializable]
    class Parte
    {
        public Dictionary<string,Poligono> Poligonos { get; set; }
        public Punto CenterOfMass { get; set; }

        public Parte()
        {
            Poligonos = new Dictionary<string, Poligono>();
            CenterOfMass = new Punto();
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
            Vector3 CenterOfMassVector = new Vector3(CenterOfMass.X, CenterOfMass.Y, CenterOfMass.Z);
            return Matrix4.CreateTranslation(-CenterOfMassVector)
                 * Matrix4.CreateTranslation(CenterOfMassVector);
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
