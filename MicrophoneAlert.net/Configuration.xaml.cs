using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MicrophoneAlert.net
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : INotifyPropertyChanged
    {
        private InputDevice selectedDevice;
        private OutputDevice selectedOutputDevice;



        public Configuration()
        {
            InitializeComponent();
            DataContext = this;
            selectedDevice = Devices.FirstOrDefault(d => d.Id == AudioDevices.Instance.SelectedDeviceId);
            selectedOutputDevice = OutputDevices.FirstOrDefault(d => d.Id == AudioDevices.Instance.SelectedOutputDeviceId);
        }

        public IEnumerable<InputDevice> Devices { get => AudioDevices.Instance.Devices; }
        public InputDevice SelectedDevice
        {
            get
            {
                return selectedDevice;
            }
            set
            {
                if (selectedDevice == value) return;
                selectedDevice = value;
                AudioDevices.Instance.SelectedDeviceId = selectedDevice.Id;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDevice)));
            }
        }


        public IEnumerable<OutputDevice> OutputDevices { get => AudioDevices.Instance.OutputDevices; }
        public OutputDevice SelectedOutputDevice
        {
            get
            {
                return selectedOutputDevice;
            }
            set
            {
                if (selectedOutputDevice == value) return;
                selectedOutputDevice = value;
                AudioDevices.Instance.SelectedOutputDeviceId = selectedOutputDevice.Id;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOutputDevice)));
            }
        }



        public int Limit
        {
            get
            {
                return AudioDevices.Instance.Limit;
            }
            set
            {
                AudioDevices.Instance.Limit = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            AudioDevices.Instance.SaveSettings();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
