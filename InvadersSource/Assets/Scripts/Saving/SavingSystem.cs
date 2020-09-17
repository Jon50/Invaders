using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Invaders.Save
{
    public static class SavingSystem
    {
        public static void SaveValue(string saveFile, object data)
        {
            using (FileStream stream = File.Open(GetSavePath(saveFile), FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
        }


        public static T LoadValue<T>(string saveFile, T defaultValue = default)
        {
            var savePath = GetSavePath(saveFile);
            if (!File.Exists(savePath)) return defaultValue;

            using (FileStream stream = File.Open(savePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }


        public static void DeleteSave(string saveFile)
        {
            if (File.Exists(GetSavePath(saveFile)))
            {
                File.Delete(GetSavePath(saveFile));
            }
        }


        public static string GetSavePath(string saveFile) => Path.Combine(Application.persistentDataPath + saveFile);
    }
}