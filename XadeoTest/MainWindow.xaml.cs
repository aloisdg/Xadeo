using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XadeoTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitTimer();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("StoryboardTest") as Storyboard;
            if (sb != null)
            {
                sb.AutoReverse = true;
                sb.Completed += sb_Completed;

                // start ticking
                _aTimer.Start();
                sb.Begin();
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            //stop ticking
            _aTimer.Stop();
        }

        private Timer _aTimer;

        public void InitTimer()
        {
            _aTimer = new Timer(200) { Enabled = true };
            _aTimer.Stop();
            _aTimer.Elapsed += aTimer_Elapsed;
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

    }
}
