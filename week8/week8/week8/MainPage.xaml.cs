using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace week8
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer _mediaPlayer = new MediaPlayer();
        MediaTimelineController _mediaTimelineController = new MediaTimelineController();
        TimeSpan _duration;
        public MainPage()
        {
            this.InitializeComponent();
            _mediaPlayer = new MediaPlayer();
            var mediaSource = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/DarkSouls.mp4"));
            mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            _mediaPlayer.Source = mediaSource;
            _mediaPlayer.CommandManager.IsEnabled = false;
            _mediaPlayer.TimelineController = _mediaTimelineController;
            _mediaPlayerElement.SetMediaPlayer(_mediaPlayer);
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            AvrilStoryboard.Begin();
            if (_mediaTimelineController.State == MediaTimelineControllerState.Paused)
            {
                _mediaTimelineController.Resume();
            }
            else
            {
                _mediaTimelineController.Start();
            }
        }

        void timer_Tick(object sender, object e)
        {
            sliderLine.Value = ((TimeSpan)_mediaTimelineController.Position).TotalSeconds;
            if (sliderLine.Value == sliderLine.Maximum)
            {
                _mediaTimelineController.Position = TimeSpan.FromSeconds(0);
            }
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            _mediaTimelineController.Pause();
            AvrilStoryboard.Pause();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            AvrilStoryboard.Stop();
            _mediaTimelineController.Position = TimeSpan.FromSeconds(0);
            _mediaTimelineController.Start();
            _mediaTimelineController.Pause();
        }

        private void ButtonFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
            _mediaPlayerElement.IsFullWindow = !_mediaPlayerElement.IsFullWindow;
        }

        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            _duration = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                sliderLine.Minimum = 0;
                sliderLine.Maximum = _duration.TotalSeconds;
                sliderLine.StepFrequency = 1;
            });
        }

        private async void ButtonFolder_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".wma");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var mediaSource = MediaSource.CreateFromStorageFile(file);
                mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
                _mediaPlayer.Source = mediaSource;
                if (file.FileType == ".mp3" || file.FileType == ".wma")
                {
                    Picture.Visibility = Visibility.Visible;
                }
                else
                {
                    Picture.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Volumn_ValueChanged(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Volume = (double)Volumn.Value;
        }

        private void ButtonVolume_Click(object sender, RoutedEventArgs e)
        {
            if(Volumn.Visibility == Visibility.Collapsed)
            {
                Volumn.Visibility = Visibility.Visible;
            }
            else
            {
                Volumn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
