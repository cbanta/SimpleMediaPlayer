using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using rv;
using SimpleMediaPlayer;
using Microsoft.Win32;
using System.Diagnostics;

namespace MediaPlayer
{

    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public MainWindow mainWindow = null;

        public List<Projector> projectors = new List<Projector>();

        public Overlay()
        {
            InitializeComponent();
            InitializeProjectors();
        }

        public void InitializeProjectors()
        {
            btnProjOn.Visibility = System.Windows.Visibility.Collapsed;
            btnProjOff.Visibility = System.Windows.Visibility.Collapsed;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(do_init_proj);
            worker.RunWorkerAsync(); 
        }

        public void do_init_proj(object sender, DoWorkEventArgs e)
        {
            RegistryKey k = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SimpleMediaPlayer");
            string proj1ip = k.GetValue("proj1", "").ToString();
            string proj2ip = k.GetValue("proj2", "").ToString();
            foreach (string pip in (new string[]{proj1ip,proj2ip}))
            {
                if (pip != "")
                {
                    try
                    {
                        Projector p = new Projector(pip);
                        p.getPower();
                        if (p.last_reponse == Command.Response.SUCCESS)
                        {
                            projectors.Add(p);
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            btnProjOn.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(update_proj_status));
        }

        public void update_proj_status()
        {
            if (projectors.Count > 0)
            {
                btnProjOn.Visibility = System.Windows.Visibility.Visible;
                btnProjOff.Visibility = System.Windows.Visibility.Visible;
                if (projectors[0].isOn)
                {
                    uiProj1Status.Fill = Brushes.Black;
                }
                else
                {
                    uiProj1Status.Fill = Brushes.White;
                }
                if (projectors.Count > 1)
                {
                    uiProj2Status.Visibility = System.Windows.Visibility.Visible;
                    if (projectors[1].isOn)
                    {
                        uiProj2Status.Fill = Brushes.Black;
                    }
                    else
                    {
                        uiProj2Status.Fill = Brushes.White;
                    }
                }
                else {
                    uiProj2Status.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                btnProjOn.Visibility = System.Windows.Visibility.Collapsed;
                btnProjOff.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void fade_in()
        {
            var anim = new System.Windows.Media.Animation.DoubleAnimation(0.5, (Duration)TimeSpan.FromSeconds(0.05));
            this.BeginAnimation(UIElement.OpacityProperty, anim);
            Mouse.OverrideCursor = null;
        }
        public void fade_out()
        {
            var anim = new System.Windows.Media.Animation.DoubleAnimation(0.0, (Duration)TimeSpan.FromSeconds(0.05));
            this.BeginAnimation(UIElement.OpacityProperty, anim);
            Mouse.OverrideCursor = Cursors.None;
        }
        public void fade_dim()
        {
            var anim = new System.Windows.Media.Animation.DoubleAnimation(0.05, (Duration)TimeSpan.FromSeconds(0.05));
            this.BeginAnimation(UIElement.OpacityProperty, anim);
            Mouse.OverrideCursor = null;
        }

        public void load(string fname)
        {
            mainWindow.player.URL = fname;
            mainWindow.player.Ctlcontrols.pause();
            fade_dim();
        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                load(ofd.FileName);
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.player.Ctlcontrols.play();
            fade_out();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.player.Ctlcontrols.pause();
            fade_dim();
        }

        private void btnRewind_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.player.Ctlcontrols.play();
            mainWindow.player.Ctlcontrols.currentPosition = 0;
            mainWindow.player.Ctlcontrols.pause();
            fade_dim();
        }

        private void btnProjOn_Click(object sender, RoutedEventArgs e)
        {
            foreach (Projector p in projectors){
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(do_proj_on);
                worker.RunWorkerAsync(p);
            }
        }

        public void do_proj_on(object sender, DoWorkEventArgs e)
        {
            Projector p = (Projector)e.Argument;
            p.turnOn();
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(update_proj_status));
        }

        private void btnProjOff_Click(object sender, RoutedEventArgs e)
        {
            foreach (Projector p in projectors)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(do_proj_off);
                worker.RunWorkerAsync(p);
            }
        }

        public void do_proj_off(object sender, DoWorkEventArgs e)
        {
            Projector p = (Projector)e.Argument;
            p.turnOff();
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(update_proj_status));
        }
    }
}
