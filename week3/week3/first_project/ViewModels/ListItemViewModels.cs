using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using first_project.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace first_project.ViewModels
{
    public class ListItemViewModels
    {
        private ObservableCollection<Models.ListItem> allItems = new ObservableCollection<Models.ListItem>();

        public ObservableCollection<Models.ListItem> AllItems { get { return this.allItems; } }
        public void AddTodoItem(string title,string description,DateTimeOffset date,BitmapImage src)
        {
            this.allItems.Add(new Models.ListItem(title, description,date,src));
        }

        public void RemoveTodoItem(string id)
        {
            ListItem temp = new ListItem();
            for(int i = 0; i < allItems.Count(); i++)
            {
                if(allItems[i].id == id)
                {
                    temp = allItems[i];
                    break;
                }
            }
            this.allItems.Remove(temp);
        }

        public void UpdateTodoItem(string id,string title,string description,DateTimeOffset date,BitmapImage src)
        {
            /*ListItem temp = new ListItem();
            for (int i = 0; i < allItems.Count(); i++)
            {
                if (allItems[i].id == id)
                {
                    allItems[i].title = title;
                    allItems[i].description = description;
                    allItems[i].date = date;
                    allItems[i].src = src;
                    break;
                }
            }*/
            this.RemoveTodoItem(id);
            this.AddTodoItem(title, description, date, src);
        }
    }
}
