using System;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;


using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GreenScreenMagic.Resources;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Nokia.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace GreenScreenMagic
{
    public partial class MainPage : PhoneApplicationPage
    {
        private BitmapImage _bgImage;
        private BitmapImage _resultImage;
        private BitmapImage _imageToEdit;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.ImageToEdit != null)
                selectImageBrush.ImageSource = App.ImageToEdit;
            if (App.ImageForBackground != null)
                selectBGBrush.ImageSource = App.ImageForBackground;
            if (App.Result != null)
                viewResultBrush.ImageSource = App.Result;
        }
        private void result_click(object sender, RoutedEventArgs e)
        {
            if (App.GSModel.isReady())
                NavigationService.Navigate(new Uri("/ResultPage.xaml", UriKind.Relative));
        }

        private void select_image_click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed += new EventHandler<PhotoResult>(imageChooserTaskCompleted);
            photo.ShowCamera = true;
            photo.Show();
        }

        private void select_bg_click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed += new EventHandler<PhotoResult>(bgChooserTaksCompleted);
            photo.ShowCamera = true;
            photo.Show();
        }

        private void imageChooserTaskCompleted(object sender, PhotoResult e) 
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MemoryStream stream = new MemoryStream();
                e.ChosenPhoto.CopyTo(stream);

                App.GSModel.ImageBuffer = stream.GetWindowsRuntimeBuffer();

                e.ChosenPhoto.Flush();
                _imageToEdit = new BitmapImage();
                _imageToEdit.SetSource(e.ChosenPhoto);

                App.OriginalFileName = e.OriginalFileName;
                App.ImageToEdit = _imageToEdit;
                NavigationService.Navigate(new Uri("/ChromaSelectPage.xaml", UriKind.Relative));
            }
        }

        //background choosed
        private void bgChooserTaksCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MemoryStream stream = new MemoryStream();
                e.ChosenPhoto.CopyTo(stream);
                App.GSModel.BackgroundBuffer = stream.GetWindowsRuntimeBuffer();

                e.ChosenPhoto.Flush();
                _bgImage = new BitmapImage();
                _bgImage.SetSource(e.ChosenPhoto);
                App.ImageForBackground = _bgImage;

                if(App.GSModel.isReady())
                    NavigationService.Navigate(new Uri("/ResultPage.xaml", UriKind.Relative));
            }
        }
    }
}