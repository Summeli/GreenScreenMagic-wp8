using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace GreenScreenMagic
{
    public partial class ResultPage : PhoneApplicationPage
    {
        public ResultPage()
        {
            InitializeComponent();
        }

        async protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap((int)App.ImageToEdit.PixelWidth, (int)App.ImageToEdit.PixelHeight);

            await App.GSModel.RenderResultBitmap(writeableBitmap);
            imageBrush.ImageSource = writeableBitmap;
            App.Result = writeableBitmap;
        }

        async public void save_clicked(object sender, RoutedEventArgs e)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap((int)App.ImageToEdit.PixelWidth, (int)App.ImageToEdit.PixelHeight);

            await App.GSModel.RenderResultBitmap(writeableBitmap);

            //TODO: saving the bitmap doesn't work
            var library = new MediaLibrary();
            using (MemoryStream s = new MemoryStream())
            {
                writeableBitmap.SaveJpeg(s, (int)App.ImageToEdit.PixelWidth, (int)App.ImageToEdit.PixelHeight, 0, 100);
                //library.SavePictureToCameraRoll(App.OriginalFileName + "-edited", s);
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }

    }
}