using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using System.Net.Http;
using week7;
// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace week7
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void searchWeather_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RootObject myWeather = await OpenWeatherMapProxy.GetWeather(JsonlocationInput.Text);

                JsonweatherResult.Text = myWeather.results[0].location.name + "的温度是" + myWeather.results[0].now.temperature;
            }
            catch
            {

            }
        }

        private async void XmlsearchWeather_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var XmlmyWeather = await xmlway.GetWeather(XmllocationInput.Text);
                XmlweatherResult.Text = XmlmyWeather.results.currentCity + "的PM2.5是 " + XmlmyWeather.results.pm25;
            }
            catch
            {

            }
        }
    }
}
