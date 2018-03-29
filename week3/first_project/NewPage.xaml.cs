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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace first_project
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public bool isCreate=true;
        public Content temp;
        public NewPage()
        {
            temp = new Content();
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
                    temp.result.AddTodoItem(TitleBlock.Text, DetailBlock.Text, Date.Date);
                    this.Frame.Navigate(typeof(MainPage), temp.result);
                }
                else
                {
                    temp.result.UpdateTodoItem(temp.id, TitleBlock.Text, DetailBlock.Text, Date.Date);
                    this.Frame.Navigate(typeof(MainPage), temp.result);
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
            base.OnNavigatedTo(e);
            temp = (Content)e.Parameter;
            if(temp.result.AllItems.Count() > 0 && temp.id != "NULL")
            {
                Create_To_Update();
                
                ListItem result = new ListItem("", "", DateTimeOffset.Now);
                for (int i = 0; i < temp.result.AllItems.Count(); i++)
                {
                    if (temp.id == temp.result.AllItems[i].id)
                    {
                        result = temp.result.AllItems[i];
                        break;
                    }
                }
                TitleBlock.Text = result.title;
                DetailBlock.Text = result.description;
                Date.Date = result.date;

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
    }
}
