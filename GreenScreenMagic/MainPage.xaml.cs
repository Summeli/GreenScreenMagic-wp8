using System;
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

namespace GreenScreenMagic
{
    public partial class MainPage : PhoneApplicationPage
    {
        private BitmapImage _imageToEdit;
        private BitmapImage _bgImage;
        private BitmapImage _resultImage;
        private StreamImageSource _imgStreamSource;
        private MemoryStream _imgStream;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void result_click(object sender, RoutedEventArgs e)
        {
        }

        private void select_image_click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed += new EventHandler<PhotoResult>(imageChooserTaksCompleted);
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

        private void imageChooserTaksCompleted(object sender, PhotoResult e) 
        {
            if (e.TaskResult == TaskResult.OK)
            {
                _imageToEdit = new BitmapImage();
                _imageToEdit.SetSource(e.ChosenPhoto);
                using (MemoryStream _imgStream = new MemoryStream())   
                 {
                     e.ChosenPhoto.CopyTo(_imgStream);
                     _imgStreamSource = new StreamImageSource(_imgStream);
                 }
                //save the image into application service before going forward
                PhoneApplicationService.Current.State["imageToEdit"] = _imageToEdit;
                PhoneApplicationService.Current.State["imageStream"] = _imgStreamSource;
                NavigationService.Navigate(new Uri("/ChromaSelectPage.xaml", UriKind.Relative)); 
            }
        }

        //background choosed
        private void bgChooserTaksCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                _imageToEdit = new BitmapImage();
                _imageToEdit.SetSource(e.ChosenPhoto);
            }
        }
    }
}