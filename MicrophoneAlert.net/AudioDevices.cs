using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace MicrophoneAlert.net
{
    public class AudioDevices
    {
        #region Singleton
        private static readonly object _syncRoot = new object();
        private static AudioDevices instance;
        public static AudioDevices Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    return instance ?? (instance = new AudioDevices());
                }
            }
        }
        #endregion

        private string selectedDeviceId;
        private string selectedOutputDeviceId;
        private int limit;

        private MMDevice selectedDevice;
        private MMDevice selectedOutputDevice;

        private SemaphoreSlim semaphore;
        private Settings settings;
        private readonly string settingPath;

        public AudioDevices()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFolder = $"{appDataFolder}\\tecnologer\\MicrophoneAlert";

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            settingPath = $"{appDataFolder}\\settings.json";
            semaphore = new SemaphoreSlim(1, 1);            
            Devices = new List<InputDevice>();
            OutputDevices = new List<OutputDevice>();
            settings = Settings.Get(settingPath);
            
            if(settings != null)
            {
                selectedDeviceId = settings.InputId;
                selectedOutputDeviceId = settings.OutputId;
                limit = settings.Limit;
            }
            else
            {
                Limit = 70;
                SelectedDevice = null;
                SelectedDeviceId = null;
                selectedOutputDeviceId= null;
            }
        }

        public List<InputDevice> Devices { get; private set; }
        public List<OutputDevice> OutputDevices { get; private set; }

        public Dispatcher Dispatcher { private get; set; }

        public string SelectedDeviceId
        {
            get
            {
                return selectedDeviceId;
            }
            set
            {
                if (selectedDeviceId == value) return;
                selectedDeviceId = value;
                if(settings != null)
                    settings.InputId = value;
                UpdateListDevices();
            }
        }

        public string SelectedOutputDeviceId
        {
            get
            {
                return selectedOutputDeviceId;
            }
            set
            {
                if (selectedOutputDeviceId == value) return;
                selectedOutputDeviceId = value;
                if (settings != null)
                    settings.OutputId = value;
                UpdateListDevices();
            }
        }

        public int Limit { get
            {
                return limit;
            }
            set
            {
                if (limit == value) return;
                limit = value;
                if (settings != null)
                    settings.Limit = value;
            }
        }

        public MMDevice SelectedDevice 
        { 
            get => selectedDevice; 
            set 
            {
                selectedDevice = value;
                if (selectedDevice != null && selectedDevice.ID != selectedDeviceId)
                {
                    selectedDeviceId = selectedDevice.ID;
                }
            } 
        }

        public MMDevice SelectedOutputDevice
        {
            get => selectedOutputDevice;
            set
            {
                selectedOutputDevice = value;
                if (selectedOutputDevice != null && selectedOutputDevice.ID != selectedOutputDeviceId)
                {
                    selectedOutputDeviceId = selectedOutputDevice.ID;
                }
            }
        }

        public void UpdateListDevices()
        {
            semaphore.WaitAsync();
            try
            {
                var enumerator = new MMDeviceEnumerator();
                var originalDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                var originalOutputDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                Devices.Clear();
                OutputDevices.Clear();
                Devices.AddRange(originalDevices.ToList().Select(d => new InputDevice(d.ID, d.FriendlyName)).ToList());
                OutputDevices.AddRange(originalOutputDevices.ToList().Select(d => new OutputDevice(d.ID, d.FriendlyName)).ToList());


                if (string.IsNullOrWhiteSpace(selectedDeviceId))
                {
                    SelectedDevice = originalDevices.First();
                    SelectedOutputDevice = originalOutputDevices.First();
                }
                else
                {
                    
                    SelectedDevice = enumerator.GetDevice(SelectedDeviceId);
                    SelectedOutputDevice = enumerator.GetDevice(selectedOutputDeviceId);
                }

#if DEBUG
                foreach (var wasapi in originalDevices)
                {
                    Debug.WriteLine($"{wasapi.ID}:{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}");
                }
#endif
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void SaveSettings()
        {
            if (settings != null)
                settings.Save();
        }

        public float GetVolume()
        {
            if (SelectedDevice == null || SelectedDevice.ID != SelectedDeviceId)
            {
                UpdateListDevices();
            }

            if (selectedDevice == null) return 0;

            return SelectedDevice != null ? SelectedDevice.AudioMeterInformation.MasterPeakValue * 100 : 0;
        }
    }
}
