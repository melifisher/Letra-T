using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Letra_T
{
    class Escenario
    {
        public Dictionary<string, Objeto> Objetos { get; private set; }
        public Punto CenterOfMass { get; private set; }
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
        public void Draw()
        {
            foreach (var objeto in Objetos)
            {
                objeto.Value.Draw();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var objeto in Objetos)
            {
                objeto.Value.Update(deltaTime);
            }
        }
    }
}