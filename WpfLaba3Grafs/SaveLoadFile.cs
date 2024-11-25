using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WpfLaba3Grafs
{
    //public class UserSettings
    //{
    //    public List<Node> graph = new List<Node>();
    //}
    [Serializable]
    public class SettingsManager
    {
        //private string SettingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\userSettings.xml";

        public void SaveSettings(List<NodeDTO> graph, string SettingsFilePath)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(graph, settings); //Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }
        public List<NodeDTO> LoadSettings(string SettingsFilePath)
        {
            if (!File.Exists(SettingsFilePath))
                throw new FileNotFoundException("Файл не найден", SettingsFilePath);

            string json = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<List<NodeDTO>>(json);
        }
    }
}
