using Newtonsoft.Json;
using System;
using System.IO;

namespace Letra_T
{
    class ObjetoSerializer
    {
        public static void Guardar<T>(T objeto, string rutaArchivo)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(objeto, Formatting.Indented);
                File.WriteAllText(rutaArchivo, jsonString);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        public static T Cargar<T>(string rutaArchivo)
        {
            string jsonString = File.ReadAllText(rutaArchivo);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
