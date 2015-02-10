using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AxWMPLib.AxWindowsMediaPlayer player = null;
        public Overlay overlay = null;

        public MainWindow()
        {
            InitializeComponent();
            InitMediaPlayer();
            InitOverlay();
            InitArgs();
            SwitchToFullScreen();

        }

        void InitArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                overlay.load(args[1]);
            }
        }

        void InitOverlay()
        {
            overlay = new Overlay();
            overlay.mainWindow = this;
            overlay.Opacity = 0.5;
            overlay.Show();
        }

        void InitMediaPlayer()
        {
            player = formsHost.Child as AxWMPLib.AxWindowsMediaPlayer;
            player.uiMode = "none";
            player.settings.setMode("loop", false);
            player.stretchToFit = true;
            player.enableContextMenu = false;
            player.ErrorEvent += new EventHandler(player_ErrorEvent);
            player.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);
            player.ClickEvent += new AxWMPLib._WMPOCXEvents_ClickEventHandler(player_ClickEvent);
            player.settings.autoStart = false;
        }

        void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                overlay.fade_dim();
                overlay.Focus();
            }
            //else if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            //{
            //    SwitchToFullScreen();
            //}
        }

        private void SwitchToFullScreen()
        {
            //topPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.WindowStyle = WindowStyle.None;
            //this.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void SwitchToNormalScreen()
        {
            //topPanel.Visibility = System.Windows.Visibility.Visible;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = System.Windows.WindowState.Normal;
        }

        void player_ErrorEvent(object sender, EventArgs e)
        {
            //deal with error here
            //messageLabel.Content = "An error occured while trying to play the video.";
        }

        void player_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            overlay.Focus();
            if (overlay.Opacity < 0.5)
            {
                player.Ctlcontrols.pause();
                overlay.fade_in();
            }
            else
            {
                overlay.fade_out();
            }
            //SwitchToNormalScreen();
        }

        //private void browseMediaButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();            
        //    ofd.CheckPathExists = true;
        //    ofd.Multiselect = true;
        //    bool? result = ofd.ShowDialog();
        //    if (result == true)
        //    {
        //        messageLabel.Content = "";
        //        player.URL = ofd.FileName;
        //        player.Ctlcontrols.play();
        //    }
        //}
    }
}
