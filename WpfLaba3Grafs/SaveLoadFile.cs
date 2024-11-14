using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfLaba3Grafs
{
    public class UserSettings
    {
        private Dictionary<int,Node> graph = new Dictionary<int,Node>();
        private List<(int,int,int)> graphData = new List<(int,int,int)> ();
        private List<EdgePicture> edgePic = new List<EdgePicture> ();
        private List<NodePicture> nodePic = new List<NodePicture> ();
    }
    [Serializable]
    public class SettingsManager
    {
        private const string SettingsFilePath = "userSettings.xml";

        public void SaveSettings(UserSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using (StreamWriter writer = new StreamWriter(SettingsFilePath))
                serializer.Serialize(writer, settings);
        }
        public UserSettings LoadSettings()
        {
            if (!System.IO.File.Exists(SettingsFilePath))
                return new UserSettings();

            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using (StreamReader reader = new StreamReader(SettingsFilePath))
                return (UserSettings)serializer.Deserialize(reader);
        }
    }
}
