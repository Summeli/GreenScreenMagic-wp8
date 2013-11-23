﻿using System;
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

            App.GSModel = new GreenScreenModel();
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
                MemoryStream stream = new MemoryStream();
                e.ChosenPhoto.CopyTo(stream);

                App.GSModel.ImageBuffer = stream.GetWindowsRuntimeBuffer();

                e.ChosenPhoto.Flush();

                _imageToEdit = new BitmapImage();
                _imageToEdit.SetSource(e.ChosenPhoto);

                App.ImageToEdit = _imageToEdit;
                NavigationService.Navigate(new Uri("/ChromaSelectPage.xaml", UriKind.Relative));


            }
        }

        //background choosed
        private void bgChooserTaksCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                _bgImage = new BitmapImage();
                _bgImage.SetSource(e.ChosenPhoto);

                MemoryStream stream = new MemoryStream();
                e.ChosenPhoto.CopyTo(stream);
                App.GSModel.BackgroundBuffer = stream.GetWindowsRuntimeBuffer();
            }
        }
    }
}