using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using WinForm = System.Windows.Forms;

namespace MicrophoneAlert.net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string decibelsValue;
        private SolidColorBrush backgroundColor;
        private Configuration config;
        private Timer timer;
        private AudioPlayer audioPlayer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            AudioDevices.Instance.Dispatcher = Dispatcher;
            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;

            audioPlayer = new AudioPlayer(); 
            TrayIcon();
        }

        public SolidColorBrush BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                if (backgroundColor?.Color == value?.Color) return;
                backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public string DecibelsValue
        {
            get
            {
                return decibelsValue;
            }
            set
            {
                if (decibelsValue == value) return;
                decibelsValue = value;
                OnPropertyChanged(nameof(DecibelsValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertiesName)
        {
            foreach (var propertyName in propertiesName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;

                Dispatcher.Invoke(delegate
                {
                    var vol = AudioDevices.Instance.GetVolume();
                    DecibelsValue = ((int)vol).ToString();

                    Color displayedColor;
                    if (vol >= AudioDevices.Instance.Limit)
                    {
                        displayedColor = Colors.Red;
                        audioPlayer.Play();
                    }
                    else
                    {
                        displayedColor = Colors.Lime;
                        audioPlayer.Stop();
                        audioPlayer.Dispose();

                    }
                    BackgroundColor = new SolidColorBrush(displayedColor);
                });
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void TrayIcon()
        {
            WinForm.NotifyIcon ni = new WinForm.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("./main.ico");
            ni.Visible = true;
            ni.ContextMenu = new WinForm.ContextMenu();
            ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("Configuration", Config_Click));
            //ni.ContextMenu.MenuItems.Add("-");
            //ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("About", About_Click));
            ni.ContextMenu.MenuItems.Add("-");
            ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("Exit", Exit_Click));            
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            if(config != null && config.IsVisible)
            {
                config.Close();
            }

            config = new Configuration();
            config.ShowDialog();
        }

        private void About_Click(object sender, EventArgs e)
        {

        }
    }
}
