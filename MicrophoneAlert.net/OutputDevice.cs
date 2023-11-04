using NAudio.CoreAudioApi;

namespace MicrophoneAlert.net
{
    public class OutputDevice
    {
        public OutputDevice(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}
