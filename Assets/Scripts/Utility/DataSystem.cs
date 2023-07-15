using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Utility
{
    public static class DataSystem
    {
        private const string directory = "SavedData";
        private const string fileName = "data.bin";


        public static void SaveBinary<T>(T[] items)
        {
            var directoryPath = Path.Combine(Application.persistentDataPath, directory);
            var filePath = Path.Combine(directory, fileName);

            bool fileExists = File.Exists(filePath);

            var path = fileExists ? filePath : directoryPath;
            var fileMode = fileExists ? FileMode.Open : FileMode.Create;
            
            var stream = new FileStream(path, fileMode);
            
            BinaryFormatter formatter = new BinaryFormatter();
            
            formatter.Serialize(stream, items);
            stream.Close();
        }
        
        public static T[] LoadBinary<T>()
        {
            var path = Path.Combine(Application.persistentDataPath, directory);

            if (!File.Exists(path))
                throw new Exception("Ensure the data saved first.");
            
            var stream = new FileStream(path, FileMode.Open);

            BinaryFormatter formatter = new BinaryFormatter();
            var items = formatter.Deserialize(stream) as T[];
            
            stream.Close();
            return items;
        }
        
        /// <summary>
        /// For debug purposes only.
        /// </summary>
        public static void SaveJson<T>(T[] items, string path)
        {
            var fileMode = File.Exists(path) ? FileMode.Open : FileMode.Create;
            
            var stream = new FileStream(path, fileMode);
            var writer = new StreamWriter(stream);
            
            for (var i = 0; i < items.Length; i++)
            {
                var line = JsonUtility.ToJson(items[i]);
                writer.WriteLine(line);
            }
            
            writer.Close();
            stream.Close();
        }
    }
}