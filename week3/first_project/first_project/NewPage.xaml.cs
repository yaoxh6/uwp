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
using first_project.ViewModels;
using first_project.Models;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace first_project
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public bool isCreate=true;
        //public Content temp;
        public NewPage()
        {
            //temp = new Content();
            this.InitializeComponent();
        }

        private async void CreateButton(object sender, RoutedEventArgs e)
        {
            if(TitleBlock.Text == "" || DetailBlock.Text == "")
            {
                var dialog = new ContentDialog()
                {
                    Title = "提示",
                    Content = "Title or Deteil is empty",
                    PrimaryButtonText = "确定",
                    //SecondaryButtonText = "取消",
                    FullSizeDesired = false,
                };
                await dialog.ShowAsync();
            }
             else if(Date.Date < DateTimeOffset.Now)
            {
                var dialog = new ContentDialog()
                {
                    Title = "提示",
                    Content = "The date is smaller than today",
                    PrimaryButtonText = "确定",
                    //SecondaryButtonText = "取消",
                    FullSizeDesired = false,
                };
                await dialog.ShowAsync();
            }
            else
            {
                if (isCreate == true)
                {
                    BitmapImage result = NewImage.Source as BitmapImage;
                    ((App)App.Current).ViewModel.AddTodoItem(TitleBlock.Text, DetailBlock.Text, Date.Date, result);
                    this.Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    BitmapImage result = NewImage.Source as BitmapImage;
                    ((App)App.Current).ViewModel.UpdateTodoItem(((App)App.Current).currentId, TitleBlock.Text, DetailBlock.Text, Date.Date,result);
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Date.Date = DateTimeOffset.Now;
            TitleBlock.Text = "";
            DetailBlock.Text = "";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
                if (e.NavigationMode == NavigationMode.New)
                {
                    ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
                }
                else
                {
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPage"))
                    {
                        var composite = ApplicationData.Current.LocalSettings.Values["NewPage"] as ApplicationDataCompositeValue;
                        TitleBlock.Text = (string)composite["Title2"];
                        DetailBlock.Text = (string)composite["Details2"];
                        Date.Date = (DateTimeOffset)composite["Date2"];
                        ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
                    }
                }
            
           
                if (((App)App.Current).currentId != "NULL")
                {
                    Create_To_Update();
                    var temp = false;
                    ListItem result = new ListItem();
                    for (int i = 0; i < ((App)App.Current).ViewModel.AllItems.Count(); i++)
                    {
                        if (((App)App.Current).currentId == ((App)App.Current).ViewModel.AllItems[i].id)
                        {
                            result = ((App)App.Current).ViewModel.AllItems[i];
                            temp = true;
                            break;
                        }
                    }
                    if (temp == true)
                    {
                        TitleBlock.Text = result.title;
                        DetailBlock.Text = result.description;
                        Date.Date = result.date;
                        NewImage.Source = result.src;
                    }
                }
                else
                {
                    Update_To_Create();
                }
            
        }
        private void Create_To_Update()
        {
            isCreate = false;
            Create.Content = "Update";
        }

        private void Update_To_Create()
        {
            isCreate = true;
            Create.Content = "Create";
        }

        private async void Select_Click(object sender, RoutedEventArgs e)
        {
            //文件选择器  
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式  
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //openPicker.ViewMode = PickerViewMode.List;  
            //初始位置  
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //添加文件类型  
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var srcImage = new BitmapImage();
                    await srcImage.SetSourceAsync(stream);
                    NewImage.Source = srcImage;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) // 从该页面离开时
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["Title2"] = TitleBlock.Text;
                composite["Details2"] = DetailBlock.Text;
                composite["Date2"] = Date.Date;
                ApplicationData.Current.LocalSettings.Values["NewPage"] = composite;
            }
        }
    }
}
