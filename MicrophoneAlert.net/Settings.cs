using Newtonsoft.Json;
using System.IO;

namespace MicrophoneAlert.net
{
    public class Settings
    {
        public string InputId { get; set; }
        public string OutputId { get; set; }
        public int Limit { get; set; }
        public string FilePath { get; set; }

        public bool Save()
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath)) return false;

                using (StreamWriter file = File.CreateText(FilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, this);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Settings Get(string file)
        {
            try
            {
                var jsonText = File.ReadAllText(file);
                var settings = JsonConvert.DeserializeObject<Settings>(jsonText);
                if(settings != null)
                {
                    settings.FilePath = file;
                }
                else
                {
                    settings = new Settings() { FilePath = file };
                }

                return settings;
            }
            catch
            {
                return new Settings() { FilePath = file };
            }
        }
    }
}
