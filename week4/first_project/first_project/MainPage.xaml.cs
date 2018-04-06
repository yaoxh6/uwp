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
using first_project;
using Windows.Storage.AccessCache;



// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace first_project
{
    public sealed partial class MainPage : Page
    {
        //public Content newTemp;
        //public ListItemViewModels ViewModel;
        public bool isClick;
        //public string currentId;
        public bool isCreate = true;
        //public BitmapImage srcImage;
        public ListItemViewModels ViewModel = ((App) App.Current).ViewModel;
        public MainPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            //ViewModel = new ListItemViewModels();
            //newTemp = new Content();
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize.Width > 800)
                {
                    isClick = false;
                }
                else
                {
                    isClick = true;
                }
            };
            
            BitmapImage srcImage = NewImage.Source as BitmapImage;
            ((App)App.Current).ViewModel.AddTodoItem("First", "Just the First", DateTimeOffset.Now, srcImage);
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

        private void addBarClick(object sender, RoutedEventArgs e)
        {
            if (isClick == true)
            {
                //Content result = new Content("NULL", ((App)App.Current).ViewModel);
                this.Frame.Navigate(typeof(NewPage));
            }
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).ViewModel.RemoveTodoItem(((App)App.Current).currentId);
            Delete.Visibility = Visibility.Collapsed;
            Update_To_Create();
        }

        private async void CreateButton(object sender, RoutedEventArgs e)
        {
            if (TitleBlock.Text == "" || DetailBlock.Text == "")
            {
                var dialog = new ContentDialog()
                {
                    Title = "提示",
                    Content = "Title or Deteil is empty",
                    PrimaryButtonText = "确定",
                    FullSizeDesired = false,
                };
                await dialog.ShowAsync();
            }
            else if (Date.Date < DateTimeOffset.Now)
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
                    var dialog = new ContentDialog()
                    {
                        Title = "提示",
                        Content = "Create Sucessfully",
                        PrimaryButtonText = "确定",
                        //SecondaryButtonText = "取消",
                        FullSizeDesired = false,
                    };
                    await dialog.ShowAsync();
                }
                else
                {
                    BitmapImage result = NewImage.Source as BitmapImage;
                    ((App)App.Current).ViewModel.UpdateTodoItem(((App)App.Current).currentId, TitleBlock.Text, DetailBlock.Text, Date.Date, result);
                    Update_To_Create();
                    var dialog = new ContentDialog()
                    {
                        Title = "提示",
                        Content = "Update Successfully",
                        PrimaryButtonText = "确定",
                        //SecondaryButtonText = "取消",
                        FullSizeDesired = false,
                    };
                    await dialog.ShowAsync();
                }
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Date.Date = DateTimeOffset.Now;
            TitleBlock.Text = "";
            DetailBlock.Text = "";
        }

        public void TodoTtem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            var temp = (ListItem)e.ClickedItem;
            ((App)App.Current).currentId = temp.id;
            if (Window.Current.Bounds.Width > 800)
            {
                Create_To_Update();
                Delete.Visibility = Visibility.Visible;
                TitleBlock.Text = temp.title;
                DetailBlock.Text = temp.description;
                Date.Date = temp.date;
                NewImage.Source = temp.src;
            }
            else
            {
                //Content result = new Content(currentId, ViewModel);
                this.Frame.Navigate(typeof(NewPage));
            }
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
                ApplicationData.Current.LocalSettings.Values["TempImage"] = StorageApplicationPermissions.FutureAccessList.Add(file);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var srcImage = new BitmapImage();
                    await srcImage.SetSourceAsync(stream);
                    NewImage.Source = srcImage;
                }
            }
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["Title"] = TitleBlock.Text;
                composite["Details"] = DetailBlock.Text;
                composite["Date"] = Date.Date;
                composite["Visible"] = ((App)App.Current).ViewModel.AllItems[0].Completed;
                ApplicationData.Current.LocalSettings.Values["MainPage"] = composite;
            }
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values["TempImage"] != null)
            {
                StorageFile tempimg;
                tempimg = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["TempImage"]);
                IRandomAccessStream ir = await tempimg.OpenAsync(FileAccessMode.Read);
                BitmapImage bi = new BitmapImage();
                await bi.SetSourceAsync(ir);
                NewImage.Source = bi;
                ApplicationData.Current.LocalSettings.Values["TempImage"] = null;
            }

                if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("MainPage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MainPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["MainPage"] as ApplicationDataCompositeValue;
                    TitleBlock.Text = (string)composite["Title"];
                    DetailBlock.Text = (string)composite["Details"];
                    Date.Date = (DateTimeOffset)composite["Date"];
                    ((App)App.Current).ViewModel.AllItems[0].Completed = (bool)composite["Visible"];
                    ApplicationData.Current.LocalSettings.Values.Remove("MainPage");
                }
            }
        }
    }
}
