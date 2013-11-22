using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GreenScreenHelper.Resources;
using Windows.Phone.Media.Capture;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Nokia.Graphics.Imaging;
using Windows.UI.Input;
using Windows.UI;
using System.Windows.Input;
using GreenScreenTools;

namespace GreenScreenHelper
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Camera
        private PhotoCaptureDevice _photoDevice = null;

        private AlphaFiltteredImageSource _filtteredImageSource = null;
        private MediaElement _mediaElement = null;
        private  Windows.UI.Color _currentColor = new Windows.UI.Color();
        private double _currentDelta;
        
        private CameraPreviewImageSource _cameraImageSource = null;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeCamera(CameraSensorLocation.Back);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Free camera resource
            _photoDevice.Dispose();
            _photoDevice = null;

        }

        private async void InitializeCamera(CameraSensorLocation sensorLocation)
        {
            // Open camera device
            _photoDevice = await PhotoCaptureDevice.OpenAsync(sensorLocation, new Windows.Foundation.Size(640, 480));

            _cameraImageSource = new CameraPreviewImageSource(_photoDevice);

            _filtteredImageSource = new AlphaFiltteredImageSource(_cameraImageSource, new Windows.Foundation.Size(640, 480));

            _mediaElement = new MediaElement();
            _mediaElement.Stretch = Stretch.UniformToFill;
            _mediaElement.BufferingTime = new TimeSpan(0);
            _mediaElement.SetSource(_filtteredImageSource);

            // Display camera viewfinder data in videoBrush element
            viewfinderBrush.SetSource(_mediaElement);
        }

        private void btn_ChangeColor(object sender, RoutedEventArgs e)
        { 
        }

        private void viewfinderCanvas_Tap(object sender, GestureEventArgs e)
        {
            Type t = e.GetType();

            _filtteredImageSource.pause();
            int height = (int) viewfinderCanvas.ActualHeight;
            int width = (int)viewfinderCanvas.ActualWidth;
            var bitmap = new WriteableBitmap(width, height);
            bitmap.Render(viewfinderCanvas, null);
            bitmap.Invalidate();
           
            Point pt = e.GetPosition(viewfinderCanvas);

            //read the pixel color at pointer position
            int[] Pixels = bitmap.Pixels;
            int pos = (int) pt.X * (int) pt.Y;
            int px = Pixels[pos];
            byte[] colorArray   = BitConverter.GetBytes(px);
            byte blue           = colorArray[0];
            byte green          = colorArray[1];
            byte red            = colorArray[2];
            byte alpha          = colorArray[3];
            _currentColor = Windows.UI.Color.FromArgb(alpha, red, green, blue);

            _filtteredImageSource.SetFilterSettings(_currentColor, _currentDelta);
            _filtteredImageSource.resume();
        }
    }
}