using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Gif.Components;
using Timer = System.Timers.Timer;

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
            _counter = 0;
            _visual = Main;
            _visualWidth = (int)Main.ActualWidth;
            _visualHeight = (int)Main.ActualHeight;
        }

        private Visual _visual;
        private int _visualWidth;
        private int _visualHeight;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = this.FindResource("StoryboardTest") as Storyboard;
            if (sb != null)
            {
                sb.Completed += sb_Completed;

                // start ticking
                _aTimer.Start();
                Thread.Sleep(200);
                sb.Begin();
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            //stop ticking
            _aTimer.Stop();
            BuildGif();
        }

        private void BuildGif()
        {
            /* create Gif */
            //you should replace filepath
            var imageFilePaths = new String[30];
            for (var i = 0; i < 15; i++)
                imageFilePaths[i] = String.Format(@"C:\Users\alois\Pictures\Main_{0}.png", i + 1);

            // reverse
            for (var i = 0; i < 15; i++)
            {
                string s = String.Format(@"C:\Users\alois\Pictures\Main_{0}.png", 15 - i);
                imageFilePaths[i + 15] = s;
            }

            const string outputFilePath = @"C:\Users\alois\Pictures\Main.gif";
            var e = new AnimatedGifEncoder();
            e.Start(outputFilePath);
            e.SetDelay(200);
            //-1:no repeat,0:always repeat
            e.SetRepeat(0);
            for (int i = 0, count = imageFilePaths.Length; i < count; i++)
            {
                e.AddFrame(System.Drawing.Image.FromFile(imageFilePaths[i]));
            }
            e.Finish();
        }

        private Timer _aTimer;
        private Int16 _counter;

        public void InitTimer()
        {
            _aTimer = new Timer(200) { Enabled = true };
            _aTimer.Stop();
            _aTimer.Elapsed += aTimer_Elapsed;
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Console.WriteLine(e.SignalTime.Ticks);
            //Console.WriteLine(e.SignalTime);
            Console.WriteLine(_counter++);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ExportXamlAsPng(_visual, _visualWidth, _visualHeight,
                    String.Concat(Main.Name, "_", _counter));
            }));
        }


        void ExportXamlAsPng(Visual visual, int width, int height, string name = "image")
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);

            // The Visual to use as the source of the RenderTargetBitmap.
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new VisualBrush(visual), null, new Rect(new Point(), new Size(width, height)));
            }

            // The BitmapSource that is rendered with a Visual.
            // Windows operating system has set the default display "DPI" to 96 PPI
            var render = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            render.Render(drawingVisual);

            // Encoding the RenderBitmapTarget as a PNG file.
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(render));
            using (var stream = new FileStream(String.Format(@"{0}\{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), name), FileMode.Create))
            {
                png.Save(stream);
            }
        }
    }
}
