using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WpfLaba3Grafs
{
    public class UserSettings
    {
        public List<Node> graph = new List<Node>();
    }
    [Serializable]
    public class SettingsManager
    {
        //private string SettingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\userSettings.xml";

        public void SaveSettings(UserSettings settings, string SettingsFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using (StreamWriter writer = new StreamWriter(SettingsFilePath))
                serializer.Serialize(writer, settings);
        }
        public UserSettings LoadSettings(string SettingsFilePath)
        {
            if (!System.IO.File.Exists(SettingsFilePath))
                return new UserSettings();

            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using (StreamReader reader = new StreamReader(SettingsFilePath))
                return (UserSettings)serializer.Deserialize(reader);
        }
    }
}
