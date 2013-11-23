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

namespace GreenScreenMagic
{
    public partial class ChomaSelectPage : PhoneApplicationPage
    {
        private BitmapImage _imageToEdit;
        private double _currentDelta;
        private BitmapImageSource _imageSource;
        private StreamImageSource _imgStream = null;
        private FilterEffect _filterEffect;

        private TransparentToBlackFilter _transparentEffect = null;
        private Windows.UI.Color _currentColor = new Windows.UI.Color();
        private Windows.Foundation.Size previewSize = new Windows.Foundation.Size(800, 480); 

        private BitmapImageSource _imgSrc;
        public ChomaSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //read the image from the application service
            if (PhoneApplicationService.Current.State.ContainsKey("imageToEdit"))
            {
                // If it exists, assign the data to the application member variable.
                _imageToEdit = PhoneApplicationService.Current.State["imageToEdit"] as BitmapImage;
                imageBrush.ImageSource = _imageToEdit;
            }

            if (PhoneApplicationService.Current.State.ContainsKey("imageStream"))
            {
                // If it exists, assign the data to the application member variable.
                _imgStream = PhoneApplicationService.Current.State["imageStream"] as StreamImageSource;

                //continue using the stream
                
            }
            
        }
        //StorageFileImageSource 

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
            
            if (_imgStream != null)
                await renderEffectAsync();
        }
        private async void accuracySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _currentDelta = e.NewValue;
            if(_imgStream!=null)
             await renderEffectAsync();
        }

        private async Task renderEffectAsync()
        {
            var filters = new List<IFilter>();
         //   filters.Add(new ChromaKeyFilter(color, distance, 1, false));
             filters.Add(new CartoonFilter(false));
            _filterEffect = new FilterEffect(_imgStream)
            {
                Filters = filters
            };

            _transparentEffect = new TransparentToBlackFilter(_filterEffect);

            WriteableBitmap writeableBitmap = new WriteableBitmap((int)previewSize.Width, (int)previewSize.Height);

            //using (BufferImageSource source = new BufferImageSource(_imgStream))
            using (FilterEffect effect = new FilterEffect(_imgStream) { Filters = filters })
            using (TransparentToBlackFilter effect2 = new TransparentToBlackFilter(effect))
            using (WriteableBitmapRenderer renderer = new WriteableBitmapRenderer(_transparentEffect, writeableBitmap))
            {
                await renderer.RenderAsync();

                writeableBitmap.Invalidate();
                //imageBrush.ImageSource = writeableBitmap;
            }
        }

    }

}