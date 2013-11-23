using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
using Windows.Storage;

namespace GreenScreenMagic
{
    public partial class ChomaSelectPage : PhoneApplicationPage
    {
        private BitmapImage _imageToEdit;
        private double _currentDelta;
        private Windows.UI.Color _currentColor = new Windows.UI.Color();

        private Windows.Foundation.Size previewSize = new Windows.Foundation.Size(800, 480);
       
        public ChomaSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //read the image from the application service
          _imageToEdit = App.ImageToEdit;
           imageBrush.ImageSource = _imageToEdit;

            
        }

        private async void imageCanvas_Tapped(object sender, GestureEventArgs e)
        {
            Type t = e.GetType();

            int height = (int)imageCanvas.ActualHeight;
            int width = (int)imageCanvas.ActualWidth;
            var bitmap = new WriteableBitmap(width, height);
            bitmap.Render(imageCanvas, null);
            bitmap.Invalidate();
            Point pt = e.GetPosition(imageCanvas);

            //read the pixel color at pointer position
            int[] Pixels = bitmap.Pixels;
            int pos = (int)pt.X * (int)pt.Y;
            int px = Pixels[pos];
            byte[] colorArray = BitConverter.GetBytes(px);
            byte blue = colorArray[0];
            byte green = colorArray[1];
            byte red = colorArray[2];
            byte alpha = colorArray[3];
            _currentColor = Windows.UI.Color.FromArgb(alpha, red, green, blue);

            if (_imageToEdit != null)
                await renderEffectAsync();
        }

        private async void accuracySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _currentDelta = e.NewValue;
            if (_imageToEdit != null)
             await renderEffectAsync();
        }

        private void done_clicked(object sender, RoutedEventArgs e)
        {
            if (App.GSModel.isReady())
                NavigationService.Navigate(new Uri("/ResultPage.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private async Task renderEffectAsync()
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap((int)_imageToEdit.PixelWidth, (int)_imageToEdit.PixelHeight);

            await App.GSModel.RenderBitmapTransparentAndBlackAsync(writeableBitmap,_currentColor,_currentDelta);
            imageBrush.ImageSource = writeableBitmap;
        }

    }

}