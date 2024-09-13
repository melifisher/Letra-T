using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Letra_T
{
    class Escenario
    {
        public Dictionary<string, Objeto> Objetos { get; private set; }
        public Shader Shader { get; private set; }
        public Camera Camera { get; private set; }
        public Punto CenterOfMass { get; private set; }
        public Escenario(Shader shader, Camera camera)
        {
            Objetos = new Dictionary<string, Objeto>();
            Shader = shader;
            Camera = camera;
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

        public void Render()
        {
            Shader.Use();

            foreach (var objeto in Objetos)
            {
                RenderObjeto(objeto.Value);
            }
        }

        private void RenderObjeto(Objeto objeto)
        {
            var objetoModelMatrix = objeto.GetModelMatrix();

            foreach (var parte in objeto.Partes)
            {
                var parteModelMatrix = parte.Value.GetModelMatrix() * objetoModelMatrix;
                Shader.SetMatrix4("model", parteModelMatrix);
                Shader.SetMatrix4("view", Camera.GetViewMatrix());
                Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

                foreach (var poligono in parte.Value.Poligonos)
                {
                    poligono.Value.Render();
                }
            }
        }

        public void Dispose()
        {   
            foreach (var objeto in Objetos)
            {
                objeto.Value.Dispose();
            }
        }
    }
}